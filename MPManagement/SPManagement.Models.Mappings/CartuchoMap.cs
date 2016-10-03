using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Models.Mappings
{
    public class CartuchoMap : EntityTypeConfiguration<Cartucho>
    {
        public CartuchoMap()
        {
            this.ToTable("Cartuchos");
            this.HasKey(c => c.Id);
            this.Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(c => c.FechaEntrada).IsRequired();
            this.Property(c => c.FechaSalida).IsRequired();
            this.Property(c => c.FechaRecepcion).IsRequired();
            this.Property(c => c.Estado).IsRequired();
            this.Property(c => c.RefrigeradorId).IsRequired();

            this.HasRequired(c => c.Refrigerador).WithMany(r => r.Cartuchos).HasForeignKey(r => r.RefrigeradorId);

        }

    }
}
