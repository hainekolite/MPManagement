namespace SPManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class Refrigerador : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _numeroDeRefrigerador;
        public string NumeroDeRefrigerador
        {
            get
            {
                return (_numeroDeRefrigerador);
            }
            set
            {
                _numeroDeRefrigerador = value;
            }
        }

        public virtual ICollection<Cartucho> Cartuchos { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
       
}