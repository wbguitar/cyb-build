using CYB_Build.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskLib;
using Xceed.Wpf.Toolkit.PropertyGrid;
using static CYB_Build.Model.TaskProcess;

namespace CYB_Build.ViewModel
{
    public class BindingManager : IDisposable
    {
        public BindingManager(TaskProcess bp)
        {
            this.bp = bp;
        }

        ~BindingManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            Cleanup();
        }

        private TaskProcess bp;
        private List<PropertyChangedEventHandler> propHandlers = new List<PropertyChangedEventHandler>();
        private List<PropertyValueChangedEventHandler> propValueHandlers = new List<PropertyValueChangedEventHandler>();
        private void Cleanup()
        {
            propHandlers.ForEach(h => ConfigVM.Instance.PropertyChanged -= h);
            propValueHandlers.ForEach(h => ConfigVM.Instance.PropertyValueChanged -= h);
        }

        public void SetupBindings(DefinitionTable procDef)
        {
            Cleanup();

            foreach (var def in procDef)
            {
                var cfg = def.Key;
                var bi = def.Value;
                foreach (var bind in bi.Bindings)
                {
                    //var h1 = new PropertyChangedEventHandler((s, e) =>
                    //{
                    //    if (e.PropertyName != "Config")
                    //        return;

                    //    var source = ConfigsVM.Instance.Configs.FirstOrDefault(c => c.Id == bind.SourceId);
                    //    if (source == ConfigVM.Instance.Config)
                    //    {
                    //        // copiamo il valore della proprieta` in base alle impostazioni del binding
                    //        var srcProp = source.GetType().GetProperty(bind.SourceProp);
                    //        var destProp = cfg.GetType().GetProperty(bind.LocalProp);

                    //        if (destProp.PropertyType != srcProp.PropertyType)
                    //            throw new InvalidOperationException($"Type mismatch: local property of type {destProp.PropertyType}, source property of type {srcProp.PropertyType}");

                    //        var val = srcProp.GetValue(source);
                    //        destProp.SetValue(cfg, val);
                    //    }
                    //});
                    //ConfigVM.Instance.PropertyChanged += h1;
                    //propHandlers.Add(h1);

                    bool insideHandler = false;
                    var h2 = new PropertyValueChangedEventHandler((s, e) =>
                    {
                        var config = s as AConfig;
                        var pi = e.OriginalSource as PropertyItem;
                        if (pi == null
                        || (pi.PropertyName != bind.SourceProp // source => local
                        && pi.PropertyName != bind.LocalProp)) // local => source
                            return;
                        var id = cfg.Id;
                        // source => local
                        if (bind.SourceId == config.Id)
                        {
                            // copiamo il valore della proprieta` in base alle impostazioni del binding
                            var srcProp = config.GetType().GetProperty(bind.SourceProp);
                            if (srcProp == null)
                                throw new InvalidOperationException($"Property {bind.SourceProp} not found in object of type { config.GetType().FullName}");

                            var destProp = cfg.GetType().GetProperty(bind.LocalProp);
                            if (destProp == null)
                                throw new InvalidOperationException($"Property {bind.LocalProp} not found in object of type {cfg.GetType().FullName}");

                            if (destProp.PropertyType != srcProp.PropertyType)
                                throw new InvalidOperationException($"Type mismatch: local property of type {destProp.PropertyType}, source property of type {srcProp.PropertyType}");

                            var val = srcProp.GetValue(config);
                            var oldval = destProp.GetValue(cfg);
                            destProp.SetValue(cfg, val);

                            if (!insideHandler)
                            {
                                //pi.DisplayName = bind.SourceProp;
                                var ea = new PropertyValueChangedEventArgs(e.RoutedEvent, pi, oldval, val);
                                ConfigVM.Instance.RaisePropertyValueChanged(cfg, ea);
                            }
                        }
                        // local => source
                        else if (cfg.Id == config.Id)
                        {
                            var src = ConfigsVM.Instance.Configs.FirstOrDefault(c => c.Id == bind.SourceId);
                            if (src == null)
                                throw new InvalidOperationException($"Can't find config with id {bind.SourceId}");

                            var localProp = config.GetType().GetProperty(bind.LocalProp);
                            if (localProp == null)
                                throw new InvalidOperationException($"Property {bind.LocalProp} not found in object of type {config.GetType().FullName}");

                            var srcProp = src.GetType().GetProperty(bind.SourceProp);
                            if (srcProp == null)
                                throw new InvalidOperationException($"Property {bind.SourceProp} not found in object of type { src.GetType().FullName}");

                            if (localProp.PropertyType != srcProp.PropertyType)
                                throw new InvalidOperationException($"Type mismatch: local property of type {localProp.PropertyType}, source property of type {srcProp.PropertyType}");

                            var val = localProp.GetValue(config);
                            var oldval = srcProp.GetValue(src);
                            srcProp.SetValue(src, val);

                            if (!insideHandler)
                            {
                                pi.DisplayName = bind.SourceProp;
                                var ea = new PropertyValueChangedEventArgs(e.RoutedEvent, pi, oldval, val);
                                insideHandler = true;
                                ConfigVM.Instance.RaisePropertyValueChanged(src, ea);
                                insideHandler = false;
                            }
                        }
                    });

                    ConfigVM.Instance.PropertyValueChanged += h2;
                    propValueHandlers.Add(h2);
                }
            }
        }
    }


}
