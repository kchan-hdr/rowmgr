﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using DailyActivitySummary.Dal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;


namespace DailyActivitySummary.Dal.Configurations
{
    public partial class ContactInfoConfiguration : IEntityTypeConfiguration<ContactInfo>
    {
        public void Configure(EntityTypeBuilder<ContactInfo> entity)
        {
            entity.HasKey(e => e.ContactId)
                .HasName("PK_ROWM.ContactInfo");

            entity.Property(e => e.ContactId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.ContactOwner)
                .WithMany(p => p.ContactInfo)
                .HasForeignKey(d => d.ContactOwnerId)
                .HasConstraintName("FK_ROWM.ContactInfo_ROWM.Owner_ContactOwnerId");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<ContactInfo> entity);
    }
}
