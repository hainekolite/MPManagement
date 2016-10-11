﻿using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Data.Repositories
{
    public class BitacoraDeMovimientosRepository : GenericRepository<BitacoraDeMovimientos>
    {
        public BitacoraDeMovimientosRepository(SPManagementDbContext context) : base(context)
        {

        }
    }
}
