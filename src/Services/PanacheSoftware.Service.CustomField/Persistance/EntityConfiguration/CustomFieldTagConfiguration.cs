using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration
{
    public class CustomFieldTagConfiguration : IEntityTypeConfiguration<CustomFieldTag>
    {
        public CustomFieldTagConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CustomFieldTag> builder)
        {
            builder.ToTable("CustomFieldTag");

            builder.Property(c => c.Description).IsRequired();
            builder.Property(c => c.Name).IsRequired();
        }
    }
}
