using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MPManagement.Views.Converters
{
    public class DateTimeToBackGroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan t = DateTime.Now.Subtract((DateTime)value);
            if (t.Hours < 6 && t.Days == 0) 
                return (Brushes.Orange);
            else if (t.Days >= 0 && t.Days <= 1 && t.Hours <= 23 && t.Minutes <= 59)
                return (Brushes.Green);
            else
                return (Brushes.Red);

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
