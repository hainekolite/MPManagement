namespace SPManagement.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class Cartucho : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _numeroDeCartucho;
        public string NumeroDeCartucho
        {
            get
            {
                return (_numeroDeCartucho);
            }
            set
            {
                _numeroDeCartucho = value;
                OnPropertyChanged();
            }
        }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaRecepcion { get; set; }
        private int _estado;
        public int Estado
        {
            get
            {
                return (_estado);
            }
            set
            {
                _estado = value;
                OnPropertyChanged();
            }
        }

        public int RefrigeradorId { get; set; }
        public virtual Refrigerador Refrigerador { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}