﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using DailyActivitySummary.Dal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;


namespace DailyActivitySummary.Dal.Configurations
{
    public partial class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> entity)
        {
            entity.Property(e => e.DocumentId).HasDefaultValueSql("(newid())");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Document> entity);
    }
}
