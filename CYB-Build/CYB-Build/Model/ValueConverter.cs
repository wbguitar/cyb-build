using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CYB_Build.Model
{
    public class ValueConverter<Tfrom, TTo>: IValueConverter
    {
        private readonly Func<Tfrom, TTo> _convert = (o) => default(TTo);
        private readonly Func<TTo, Tfrom> _convertBack = (o) => default(Tfrom);

        public ValueConverter(Func<Tfrom, TTo> convert, Func<TTo, Tfrom> convertBack = null)
        {
            _convert = convert;
            _convertBack = convertBack;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _convert((Tfrom)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_convertBack == null)
                return value;

            return _convertBack((TTo)value);
        }
    }
}
