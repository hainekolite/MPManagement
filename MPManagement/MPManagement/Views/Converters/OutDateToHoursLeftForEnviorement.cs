using SPManagement.Business;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MPManagement.Views.Converters
{
    public class OutDateToHoursLeftForEnviorement : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TiempoBusiness tiempoBusiness = new TiempoBusiness();
            Tiempo tiempo = tiempoBusiness.GetAll().FirstOrDefault();

            TimeSpan t = DateTime.Now.Subtract((DateTime)value);
            int DaysInhours = t.Days * 24;
            int Hours = t.Hours;
            int Minutes = t.Minutes;
            string warning = "El periodo de ambientacion ha concluido, favor de informar a su supervisor";
            string infoOrange = string.Format("Faltan {0} HRS con {1} MIN para poder colocar en maquina DEK",(tiempo.HorasAmbientacionMin-1)-Hours,60- Minutes);
            string infoGreen = string.Format("Faltan {0} HRS con {1} MIN para concluir el lapso maximo de ambientacion",(tiempo.HorasAmbientacionMax)-(DaysInhours+Hours),60-Minutes);

            if (( Hours  + DaysInhours) < tiempo.HorasAmbientacionMin)
                return (infoOrange);
            else if ((DaysInhours + Hours) < (tiempo.HorasAmbientacionMax))
                return (infoGreen);
            else
                return (warning);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
