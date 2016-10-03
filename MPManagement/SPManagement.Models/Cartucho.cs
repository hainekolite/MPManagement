namespace SPManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class Cartucho
    {
        public int Id { get; set; }
        public string NumeroDeCartucho { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public int Estado { get; set; }

        public int RefrigeradorId { get; set; }
        public virtual Refrigerador Refrigerador { get; set; }
    }
}