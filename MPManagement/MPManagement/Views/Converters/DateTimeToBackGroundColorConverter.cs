﻿using System;
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
            int DaysInhours = t.Days * 24;
            int Hours = t.Hours;
            int Minutes = t.Minutes;
            var converter = new System.Windows.Media.BrushConverter();

            if (t.Hours < 6 && t.Days * 24 == 0)
                return ((SolidColorBrush)converter.ConvertFromString("#EF6C00"));
            else if (DaysInhours >= 0 && DaysInhours <= 24 && Hours <= 23 && Minutes <= 59)
                return ((SolidColorBrush)converter.ConvertFromString("#00695C"));
            else
                return ((SolidColorBrush)converter.ConvertFromString("#D32F2F"));

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
