namespace SPManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public class Refrigerador
    {
        public int Id { get; set; }
        public string NumeroDeRefrigerador { get; set; }

        public virtual ICollection<Cartucho> Cartuchos { get; set; }
    }
       
}