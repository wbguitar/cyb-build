using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TaskLib.Utils
{
    public static class Ext
    {
        public static T Clone<T>(this T obj)
        {
            var ms = new MemoryStream();
            using (var sr = new StreamWriter(ms))
            {
                var ser = new BinaryFormatter();
                ser.Serialize(ms, obj);
                ms.Position = 0;
                return (T)ser.Deserialize(ms);
            }
        }

        public static MemoryStream Clone(this Stream sr)
        {
            var ms = new MemoryStream();
            sr.CopyTo(ms);
            ms.Position = 0;
            return ms;
        }

        public static StreamReader ToStreamReader(this Stream sr)
        {
            return new StreamReader(sr.Clone());
        }

        public static bool Save<T>(this T cfg, string path, Type[] extratypes = null, StreamWriter swOut = null)
        {
            try
            {
                var settings = new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    OmitXmlDeclaration = true
                };

                using (var sw = XmlWriter.Create(path, settings))
                {
                    XmlSerializer ser = null;
                    if (extratypes == null)
                        ser = new XmlSerializer(typeof(T));
                    else
                        ser = new XmlSerializer(typeof(T), extratypes);
                    ser.Serialize(sw, cfg);
                }
            }
            catch (Exception e)
            {
                var msg = $"Error serializing {typeof(T).Name} type: {e.Message}";
                if (swOut != null)
                    swOut.WriteLine(msg);
                throw new Exception(msg, e);
            }

            return true;
        }

        public static T Load<T>(this T cfg, string path, Type[] extratypes = null, StreamWriter swOut = null)
        {
            try
            {
                using (var sw = XmlReader.Create(path))
                {
                    XmlSerializer ser = null;
                    if (extratypes == null)
                        ser = new XmlSerializer(typeof(T));
                    else
                        ser = new XmlSerializer(typeof(T), extratypes);
                    return (T)ser.Deserialize(sw);
                }
            }
            catch (Exception e)
            {
                var msg = $"Error deserializing {typeof(T).Name} type: {e.Message}";
                if (swOut != null)
                    swOut.WriteLine(msg);

                throw new Exception(msg, e);
            }
        }

        public static IEnumerable<Type> EnumerateTypes(this Type tbase, Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(tbase));
        }

        public static IEnumerable<Type> EnumerateTypes(this Type tbase)
        {
            return tbase.EnumerateTypes(Assembly.GetAssembly(tbase));
            //return Assembly.GetAssembly(tbase).GetTypes()
            //    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(tbase));
        }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            objects.Sort();
            return objects;
        }

        public static string GetDescriptionAttribute(this Type type)
        {
            var descriptions = (DescriptionAttribute[])
                type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptions.Length == 0)
            {
                return null;
            }
            return descriptions[0].Description;
        }

        public static Type FindType(this string typeName, Type baseType = null)
        {
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = ass.GetTypes().AsEnumerable();
                if (baseType != null)
                    types = types
                        .Where(t => t.IsSubclassOf(baseType));
                var found = types
                    .FirstOrDefault(t => t.FullName == typeName);

                if (found != null)
                    return found;
            }

            return null;
        }

        public static IEnumerable<Type> FindSubTypes(this Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(ass => ass.GetTypes().Where(t => t.IsSubclassOf(baseType)))
                .Aggregate(new List<Type>(), (seed, types) => { seed.AddRange(types); return seed; });
        }

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static string AsNullIfEmpty(this string str) => string.IsNullOrEmpty(str) ? null : str;
        public static string Defaults(this string str, string defautlStr) => str.IsNullOrEmpty() ? defautlStr : str;
    }



    public class Loader<T> where T : new()
    {
        public static string DefaultFile = "";

        public T LoadDefault() => Load(DefaultFile);
        public T SaveDefault() => Save(DefaultFile);

        public T LoadDefault(T t) => Load(t, DefaultFile);
        public T SaveDefault(T t) => Save(t, DefaultFile);

        public T Load(string fpath)
        {
            if (!File.Exists(fpath))
                throw new FileNotFoundException($"Cannot find file ({fpath})");

            var t = new T();
            return (T)t.Load(fpath);
        }

        public T Save(string fpath)
        {
            var t = new T();
            t.Save(fpath);
            return t;
        }

        public T Load(T t, string fpath)
        {
            if (!File.Exists(fpath))
                throw new FileNotFoundException($"Cannot find file ({fpath})");

            return (T)t.Load(fpath);
        }

        public T Save(T t, string fpath)
        {
            t.Save(fpath);
            return t;
        }
    }

}
