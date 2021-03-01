﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanacheSoftware.Core.Domain.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.EntityConfiguration
{
    public class CustomFieldGroupDetailConfiguration : IEntityTypeConfiguration<CustomFieldGroupDetail>
    {
        public CustomFieldGroupDetailConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CustomFieldGroupDetail> builder)
        {
            builder.ToTable("CustomFieldGroupDetail");

            builder.Property(c => c.TenantId).IsRequired();
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.SequenceNo).IsRequired();

            builder.HasOne(c => c.CustomFieldGroupHeader)
                .WithMany(d => d.CustomFieldGroupDetails)
                .HasForeignKey(c => c.CustomFieldGroupHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
