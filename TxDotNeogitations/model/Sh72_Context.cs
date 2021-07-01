using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TxDotNeogitations
{
    public partial class Sh72_Context : DbContext
    {
        public Sh72_Context()
        {
        }

        public Sh72_Context(DbContextOptions<Sh72_Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agent { get; set; }
        public virtual DbSet<ContactInfo> ContactInfo { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentActivity> DocumentActivity { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<NegotiationContacts> NegotiationContacts { get; set; }
        public virtual DbSet<NegotiationDocuments> NegotiationDocuments { get; set; }
        public virtual DbSet<NegotiationHistory> NegotiationHistory { get; set; }
        public virtual DbSet<NegotiationParcels> NegotiationParcels { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<Ownership> Ownership { get; set; }
        public virtual DbSet<Parcel> Parcel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("server=localhost;database=rowm_71;integrated security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.Property(e => e.AgentId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<ContactInfo>(entity =>
            {
                entity.HasKey(e => e.ContactId)
                    .HasName("PK_ROWM.ContactInfo");

                entity.HasIndex(e => e.ContactOwnerId)
                    .HasName("IX_ContactOwnerId");

                entity.HasIndex(e => e.OrganizationId)
                    .HasName("IX_OrganizationId");

                entity.Property(e => e.ContactId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.ContactOwner)
                    .WithMany(p => p.ContactInfo)
                    .HasForeignKey(d => d.ContactOwnerId)
                    .HasConstraintName("FK_ROWM.ContactInfo_ROWM.Owner_ContactOwnerId");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasIndex(e => e.ContactLogContactLogId)
                    .HasName("IX_ContactLog_ContactLogId");

                entity.HasIndex(e => e.DocumentPackagePackageId)
                    .HasName("IX_DocumentPackage_PackageId");

                entity.Property(e => e.DocumentId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.DocumentTypeNavigation)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.DocumentType)
                    .HasConstraintName("FK_Document_Document_Type");
            });

            modelBuilder.Entity<DocumentActivity>(entity =>
            {
                entity.HasKey(e => e.ActivityId)
                    .HasName("PK_ROWM.DocumentActivity");

                entity.HasIndex(e => e.AgentId)
                    .HasName("IX_AgentId");

                entity.HasIndex(e => e.ChildDocumentId)
                    .HasName("IX_ChildDocumentId");

                entity.HasIndex(e => e.ParentDocumentId)
                    .HasName("IX_ParentDocumentId");

                entity.Property(e => e.ActivityId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.DocumentActivity)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_ROWM.DocumentActivity_ROWM.Agent_AgentId");

                entity.HasOne(d => d.ChildDocument)
                    .WithMany(p => p.DocumentActivityChildDocument)
                    .HasForeignKey(d => d.ChildDocumentId)
                    .HasConstraintName("FK_ROWM.DocumentActivity_ROWM.Document_ChildDocumentId");

                entity.HasOne(d => d.ParentDocument)
                    .WithMany(p => p.DocumentActivityParentDocument)
                    .HasForeignKey(d => d.ParentDocumentId)
                    .HasConstraintName("FK_ROWM.DocumentActivity_ROWM.Document_ParentDocumentId");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.DocTypeName)
                    .HasName("PK_ROWM.Document_Type");

                entity.HasIndex(e => e.MilestoneCode)
                    .HasName("IX_MilestoneCode");
            });

            modelBuilder.Entity<NegotiationContacts>(entity =>
            {
                entity.Property(e => e.NegotiationContactId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ContactInfo)
                    .WithMany(p => p.NegotiationContacts)
                    .HasForeignKey(d => d.ContactInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NegotiationContacts_ContactInfo");

                entity.HasOne(d => d.Negotiation)
                    .WithMany(p => p.NegotiationContacts)
                    .HasForeignKey(d => d.NegotiationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NegotiationContacts_Negotiation_History");
            });

            modelBuilder.Entity<NegotiationDocuments>(entity =>
            {
                entity.Property(e => e.NegotiationDocumentId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.NegotiationDocuments)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NegotiationDocuments_Document");

                entity.HasOne(d => d.Negotiation)
                    .WithMany(p => p.NegotiationDocuments)
                    .HasForeignKey(d => d.NegotiationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NegotiationDocuments_Negotiation_History");
            });

            modelBuilder.Entity<NegotiationHistory>(entity =>
            {
                entity.Property(e => e.NegotiationId).ValueGeneratedOnAdd();
                entity.Property(e => e.Created).ValueGeneratedOnAdd();
                entity.Property(e => e.LastModified)
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.NegotiationHistory)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("FK_Negotiation_History_ContactInfo");

                entity.HasOne(d => d.Negotiator)
                    .WithMany(p => p.NegotiationHistory)
                    .HasForeignKey(d => d.NegotiatorId)
                    .HasConstraintName("FK_Negotiation_History_Agent");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.NegotiationHistory)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Negotiation_History_Owner");
            });

            modelBuilder.Entity<NegotiationParcels>(entity =>
            {
                entity.Property(e => e.NegotiationParcelId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.NegotiationHistory)
                    .WithMany(p => p.NegotiationParcels)
                    .HasForeignKey(d => d.NegotiationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NegotiationParcels_Negotiation_History");

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.NegotiationParcels)
                    .HasForeignKey(d => d.ParcelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NegotiationParcels_Parcel");
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasIndex(e => e.OwnerOwnerId)
                    .HasName("IX_Owner_OwnerId");

                entity.Property(e => e.OwnerId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.OwnerOwner)
                    .WithMany(p => p.InverseOwnerOwner)
                    .HasForeignKey(d => d.OwnerOwnerId)
                    .HasConstraintName("FK_ROWM.Owner_ROWM.Owner_Owner_OwnerId");
            });

            modelBuilder.Entity<Ownership>(entity =>
            {
                entity.HasIndex(e => e.OwnerId)
                    .HasName("IX_OwnerId");

                entity.HasIndex(e => e.ParcelId)
                    .HasName("IX_ParcelId");

                entity.Property(e => e.OwnershipId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Ownership)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_ROWM.Ownership_ROWM.Owner_OwnerId");

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.Ownership)
                    .HasForeignKey(d => d.ParcelId)
                    .HasConstraintName("FK_ROWM.Ownership_ROWM.Parcel_ParcelId");
            });

            modelBuilder.Entity<Parcel>(entity =>
            {
                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.HasIndex(e => e.ParcelStatusCode)
                    .HasName("IX_ParcelStatusCode");

                entity.HasIndex(e => e.RoeStatusCode)
                    .HasName("IX_RoeStatusCode");

                entity.Property(e => e.ParcelId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CountyFips)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
