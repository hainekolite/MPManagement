using SPManagement.Business;
using SPManagement.Models;
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
            TiempoBusiness tiempoBusiness = new TiempoBusiness();
            Tiempo tiempo = tiempoBusiness.GetAll().FirstOrDefault();

            TimeSpan t = DateTime.Now.Subtract((DateTime)value);
            int DaysInhours = t.Days * 24;
            int Hours = t.Hours;
            int Minutes = t.Minutes;
            var converter = new System.Windows.Media.BrushConverter();

            if ((Hours + DaysInhours) < tiempo.HorasAmbientacionMin)
                return ((SolidColorBrush)converter.ConvertFromString("#EF6C00"));
            else if ((DaysInhours + Hours) < (tiempo.HorasAmbientacionMax))
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
