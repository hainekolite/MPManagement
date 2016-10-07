namespace SPManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class Tiempo
    {
        public int Id { get; set; }
        public int HorasAmbientacionMin { get; set; }
        public int HorasAmbientacionMax { get; set; }
        public int HorasReposoTrasRetorno { get; set; }
        public int SegundosRefresco { get; set; }

    }
}