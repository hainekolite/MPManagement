using SPManagement.Models;
using SPManagement.Models.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Data
{
    public class SPManagementDbContext : DbContext
    {
        public IDbSet<Refrigerador> Refrigeradores { get; set; }
        public IDbSet<Cartucho> Cartuchos { get; set; }
        public IDbSet<Tiempo> Tiempos { get; set; }
        public IDbSet<BitacoraDeMovimientos> BitacorasDeMovimientos { get; set; }

        public SPManagementDbContext() : base("SPManagementDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new RefrigeradorMap());
            modelBuilder.Configurations.Add(new CartuchoMap());
            modelBuilder.Configurations.Add(new TiempoMap());
            modelBuilder.Configurations.Add(new BitacoraDeMovimientosMap());
        }


    }
}
