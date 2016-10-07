using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Models.Mappings
{
    public class TiempoMap : EntityTypeConfiguration<Tiempo>
    {
        public TiempoMap()
        {
            this.ToTable("Tiempos");
            this.HasKey(c => c.Id);
            this.Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.HorasAmbientacionMax).IsRequired();
            this.Property(t => t.HorasAmbientacionMin).IsRequired();
            this.Property(t => t.HorasReposoTrasRetorno).IsRequired();
            this.Property(t => t.SegundosRefresco).IsRequired();
        } 
    }
}
