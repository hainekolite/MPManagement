namespace SPManagement.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;
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
        private DateTime _fechaEntrada;
        public DateTime FechaEntrada
        {
            get
            {
                return (_fechaEntrada);
            }
            set
            {
                _fechaEntrada = value;
                OnPropertyChanged();
            }
        }

        private DateTime _fechaSalida;
        public DateTime FechaSalida
        {
            get
            {
                return (_fechaSalida);
            }
            set
            {
                _fechaSalida = value;
                OnPropertyChanged();
            }
        }

        public DateTime FechaRecepcion { get; set; }
        public DateTime FechaTerminacion { get; set; }

        private int _estado;
        public virtual int Estado
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

        private string _nombreRefrigerador;
        public string NombreRefrigerador
        {
            get
            {
                return (_nombreRefrigerador);
            }
            set
            {
                _nombreRefrigerador = value;
                OnPropertyChanged();
            }
        }

        [NotMapped]
        private Tiempo _tiempo;
        [NotMapped]
        public Tiempo Tiempo
        {
            get
            {
                return (_tiempo);
            }
            set
            {
                _tiempo = value;
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