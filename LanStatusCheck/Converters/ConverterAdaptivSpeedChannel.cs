using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LanStatusCheck.Converters
{
    public class ConverterAdaptivSpeedChannel : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Binding.DoNothing;

            if (value is double val)
            {
                var gbS = val / 1024;

                return gbS < 1 ? String.Format("{0} Kbit\\s", val) : String.Format("{0:F2} Mbit\\s", gbS);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
