using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LanStatusCheck.Converters
{
    public class ConverterLevelDataSize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Binding.DoNothing;

            if (value is long val)
            {
                var kbytes = val / 1000;

                if (kbytes < 1000)
                    return String.Format("{0} Kb", kbytes);

                var mbytes = kbytes / 1000;

                if (mbytes<1000)
                    return String.Format("{0} Mb", mbytes);

                var gbytes = mbytes / 1000.0;

                return String.Format("{0:F2} Gb", gbytes);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
