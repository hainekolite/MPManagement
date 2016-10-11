using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Models.Mappings
{
    public class BitacoraDeMovimientosMap : EntityTypeConfiguration<BitacoraDeMovimientos>
    {
        public BitacoraDeMovimientosMap()
        {
            this.ToTable("BitacoraDeMovimientos");
            this.HasKey(b => b.Id);
            this.Property(b => b.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(b => b.RefrigeradorId).IsOptional();
            this.Property(b => b.UserId).IsOptional();
            this.Property(b => b.NumeroDeCartucho).IsRequired();
            this.Property(b => b.FechaMovimiento).IsRequired();
            this.Property(b => b.TipoMovimiento).IsRequired();
        }
    }

}
