using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Models.Mappings
{
    public class RefrigeradorMap : EntityTypeConfiguration<Refrigerador>
    {
        public RefrigeradorMap()
        {
            this.ToTable("Refrigeradores");
            this.HasKey(r => r.Id);
            this.Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(r => r.NumeroDeRefrigerador).IsRequired().HasMaxLength(50).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[] { new IndexAttribute(IndexAnnotation.AnnotationName) { IsUnique = true } }));

            this.HasMany(r => r.Cartuchos);
        }
    }
}
