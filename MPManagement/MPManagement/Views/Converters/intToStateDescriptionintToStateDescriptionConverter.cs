using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MPManagement.Views.Converters
{
    public class intToStateDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
                return ("Entrada a refrigerador");
            else if ((int)value == 1)
                return ("Salida a ambientacion");
            else if ((int)value == 2)
                return ("insertada a maquina DEK");
            else if ((int)value == 3)
                return ("Retorno a refrigerador");
            else if ((int)value == 4)
                return ("Cartucho terminado");
            else
                return ("Movimiento no catalogado");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
