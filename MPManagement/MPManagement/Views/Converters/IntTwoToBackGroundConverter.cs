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
    public class IntTwoToBackGroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (SolidColorBrush)converter.ConvertFromString("#00ACC1");
            var nonBrush = (SolidColorBrush)converter.ConvertFromString("#006064");

            int option = (int)value;
            if (option.Equals(2))
                return (brush);
            else
                return (nonBrush);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
