﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Models
{
    public class BitacoraDeMovimientos
    {
        public int Id {get; set;}
        public string UserId { get; set; }
        public int RefrigeradorId { get; set; }
        public string NumeroDeCartucho { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int TipoMovimiento { get; set; }

        /*NotMappedSection*/
        [NotMapped]
        public string NombreDeEmpleado { get; set; }        
        [NotMapped]
        public string NombreRefrigerador { get; set; }

    }
}
