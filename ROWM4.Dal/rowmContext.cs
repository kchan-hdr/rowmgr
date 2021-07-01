using System;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ROWM.Dal
{
    public partial class ROWM_Context : DbContext
    {
        public ROWM_Context() { }

        public ROWM_Context(DbContextOptions<ROWM_Context> options)
            : base(options)
        {
            var conn = Database.GetDbConnection() as SqlConnection;
            conn.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/", "a4390e1c-661f-4cab-a1cd-a2a9d8508b98").Result;
        }

        public virtual DbSet<Agent> Agent { get; set; }
        public virtual DbSet<Appraisal> Appraisal { get; set; }
        public virtual DbSet<ContactChannel> ContactChannel { get; set; }
        public virtual DbSet<ContactFollowup> ContactFollowup { get; set; }
        public virtual DbSet<ContactInfo> ContactInfo { get; set; }
        public virtual DbSet<ContactInfoContactLogs> ContactInfoContactLogs { get; set; }
        public virtual DbSet<ContactLog> ContactLog { get; set; }
        public virtual DbSet<ContactPurpose> ContactPurpose { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentActivity> DocumentActivity { get; set; }
        public virtual DbSet<DocumentAgents> DocumentAgents { get; set; }
        public virtual DbSet<DocumentPackage> DocumentPackage { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<LandownerScore> LandownerScore { get; set; }
        public virtual DbSet<Map> Map { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<OwnerDocuments> OwnerDocuments { get; set; }
        public virtual DbSet<Ownership> Ownership { get; set; }
        public virtual DbSet<Parcel> Parcel { get; set; }
        public virtual DbSet<ParcelContactInfo> ParcelContactInfo { get; set; }
        public virtual DbSet<ParcelContactLogs> ParcelContactLogs { get; set; }
        public virtual DbSet<ParcelDocuments> ParcelDocuments { get; set; }
        public virtual DbSet<ParcelStatus> ParcelStatus { get; set; }
        public virtual DbSet<PreferredContactMode> PreferredContactMode { get; set; }
        public virtual DbSet<RepresentationType> RepresentationType { get; set; }
        public virtual DbSet<RoeConditions> RoeConditions { get; set; }
        public virtual DbSet<RoeStatus> RoeStatus { get; set; }
        public virtual DbSet<StatusActivity> StatusActivity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=wharton-dev-rowm.database.windows.net;Initial Catalog=rowm;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.Property(e => e.AgentId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Appraisal>(entity =>
            {
                entity.Property(e => e.AppraisalId).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_Appraisal_Agent");

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.ParcelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appraisal_Parcel");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_Appraisal_DocumentPackage");
            });

            modelBuilder.Entity<ContactChannel>(entity =>
            {
                entity.HasKey(e => e.ContactTypeCode)
                    .HasName("PK_ROWM.Contact_Channel");
            });

            modelBuilder.Entity<ContactFollowup>(entity =>
            {
                entity.HasIndex(e => e.ChildContactLogId)
                    .HasName("IX_ChildContactLogId");

                entity.HasIndex(e => e.ParentContactLogId)
                    .HasName("IX_ParentContactLogId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.ChildContactLog)
                    .WithMany(p => p.ContactFollowupChildContactLog)
                    .HasForeignKey(d => d.ChildContactLogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ROWM.Contact_Followup_ROWM.ContactLog_ChildContactLogId");

                entity.HasOne(d => d.ParentContactLog)
                    .WithMany(p => p.ContactFollowupParentContactLog)
                    .HasForeignKey(d => d.ParentContactLogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ROWM.Contact_Followup_ROWM.ContactLog_ParentContactLogId");
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

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ContactInfo)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_ROWM.ContactInfo_ROWM.Organization_OrganizationId");

                entity.HasOne(d => d.PreferredContactModeNavigation)
                    .WithMany(p => p.ContactInfo)
                    .HasForeignKey(d => d.PreferredContactMode)
                    .HasConstraintName("FK_MODE");
            });

            modelBuilder.Entity<ContactInfoContactLogs>(entity =>
            {
                entity.HasKey(e => new { e.ContactInfoContactId, e.ContactLogContactLogId })
                    .HasName("PK_ROWM.ContactInfoContactLogs");

                entity.HasIndex(e => e.ContactInfoContactId)
                    .HasName("IX_ContactInfo_ContactId");

                entity.HasIndex(e => e.ContactLogContactLogId)
                    .HasName("IX_ContactLog_ContactLogId");

                entity.HasOne(d => d.ContactInfoContact)
                    .WithMany(p => p.ContactInfoContactLogs)
                    .HasForeignKey(d => d.ContactInfoContactId)
                    .HasConstraintName("FK_ROWM.ContactInfoContactLogs_ROWM.ContactInfo_ContactInfo_ContactId");

                entity.HasOne(d => d.ContactLogContactLog)
                    .WithMany(p => p.ContactInfoContactLogs)
                    .HasForeignKey(d => d.ContactLogContactLogId)
                    .HasConstraintName("FK_ROWM.ContactInfoContactLogs_ROWM.ContactLog_ContactLog_ContactLogId");
            });

            modelBuilder.Entity<ContactLog>(entity =>
            {
                entity.HasIndex(e => e.ContactAgentId)
                    .HasName("IX_ContactAgentId");

                entity.HasIndex(e => e.OwnerOwnerId)
                    .HasName("IX_Owner_OwnerId");

                entity.Property(e => e.ContactLogId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.ContactAgent)
                    .WithMany(p => p.ContactLog)
                    .HasForeignKey(d => d.ContactAgentId)
                    .HasConstraintName("FK_ROWM.ContactLog_ROWM.Agent_ContactAgentId");

                entity.HasOne(d => d.OwnerOwner)
                    .WithMany(p => p.ContactLog)
                    .HasForeignKey(d => d.OwnerOwnerId)
                    .HasConstraintName("FK_ROWM.ContactLog_ROWM.Owner_Owner_OwnerId");
            });

            modelBuilder.Entity<ContactPurpose>(entity =>
            {
                entity.HasKey(e => e.PurposeCode)
                    .HasName("PK_ROWM.Contact_Purpose");

                entity.HasIndex(e => e.MilestoneCode)
                    .HasName("IX_MilestoneCode");

                entity.HasOne(d => d.MilestoneCodeNavigation)
                    .WithMany(p => p.ContactPurpose)
                    .HasForeignKey(d => d.MilestoneCode)
                    .HasConstraintName("FK_ROWM.Contact_Purpose_ROWM.Parcel_Status_MilestoneCode");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasIndex(e => e.ContactLogContactLogId)
                    .HasName("IX_ContactLog_ContactLogId");

                entity.HasIndex(e => e.DocumentPackagePackageId)
                    .HasName("IX_DocumentPackage_PackageId");

                entity.Property(e => e.DocumentId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.ContactLogContactLog)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.ContactLogContactLogId)
                    .HasConstraintName("FK_ROWM.Document_ROWM.ContactLog_ContactLog_ContactLogId");

                entity.HasOne(d => d.DocumentPackagePackage)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.DocumentPackagePackageId)
                    .HasConstraintName("FK_ROWM.Document_ROWM.DocumentPackage_DocumentPackage_PackageId");

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

            modelBuilder.Entity<DocumentAgents>(entity =>
            {
                entity.HasKey(e => new { e.AgentAgentId, e.DocumentDocumentId })
                    .HasName("PK_ROWM.DocumentAgents");

                entity.HasIndex(e => e.AgentAgentId)
                    .HasName("IX_Agent_AgentId");

                entity.HasIndex(e => e.DocumentDocumentId)
                    .HasName("IX_Document_DocumentId");

                entity.HasOne(d => d.AgentAgent)
                    .WithMany(p => p.DocumentAgents)
                    .HasForeignKey(d => d.AgentAgentId)
                    .HasConstraintName("FK_ROWM.DocumentAgents_ROWM.Agent_Agent_AgentId");

                entity.HasOne(d => d.DocumentDocument)
                    .WithMany(p => p.DocumentAgents)
                    .HasForeignKey(d => d.DocumentDocumentId)
                    .HasConstraintName("FK_ROWM.DocumentAgents_ROWM.Document_Document_DocumentId");
            });

            modelBuilder.Entity<DocumentPackage>(entity =>
            {
                entity.HasKey(e => e.PackageId)
                    .HasName("PK_ROWM.DocumentPackage");

                entity.Property(e => e.PackageId).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.DocTypeName)
                    .HasName("PK_ROWM.Document_Type");

                entity.HasIndex(e => e.MilestoneCode)
                    .HasName("IX_MilestoneCode");

                entity.HasOne(d => d.MilestoneCodeNavigation)
                    .WithMany(p => p.DocumentType)
                    .HasForeignKey(d => d.MilestoneCode)
                    .HasConstraintName("FK_ROWM.Document_Type_ROWM.Parcel_Status_MilestoneCode");
            });

            modelBuilder.Entity<LandownerScore>(entity =>
            {
                entity.HasKey(e => e.Score)
                    .HasName("PK_ROWM.Landowner_Score");

                entity.Property(e => e.Score).ValueGeneratedNever();
            });

            modelBuilder.Entity<Map>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.OrganizationId).HasDefaultValueSql("(newsequentialid())");
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

            modelBuilder.Entity<OwnerDocuments>(entity =>
            {
                entity.HasKey(e => new { e.DocumentDocumentId, e.OwnerOwnerId })
                    .HasName("PK_ROWM.OwnerDocuments");

                entity.HasIndex(e => e.DocumentDocumentId)
                    .HasName("IX_Document_DocumentId");

                entity.HasIndex(e => e.OwnerOwnerId)
                    .HasName("IX_Owner_OwnerId");

                entity.HasOne(d => d.DocumentDocument)
                    .WithMany(p => p.OwnerDocuments)
                    .HasForeignKey(d => d.DocumentDocumentId)
                    .HasConstraintName("FK_ROWM.OwnerDocuments_ROWM.Document_Document_DocumentId");

                entity.HasOne(d => d.OwnerOwner)
                    .WithMany(p => p.OwnerDocuments)
                    .HasForeignKey(d => d.OwnerOwnerId)
                    .HasConstraintName("FK_ROWM.OwnerDocuments_ROWM.Owner_Owner_OwnerId");
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

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.InverseParcelParcel)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_ROWM.Parcel_ROWM.Parcel_Parcel_ParcelId");

                entity.HasOne(d => d.ParcelStatusCodeNavigation)
                    .WithMany(p => p.Parcel)
                    .HasForeignKey(d => d.ParcelStatusCode)
                    .HasConstraintName("FK_ROWM.Parcel_ROWM.Parcel_Status_ParcelStatusCode");

                entity.HasOne(d => d.RoeStatusCodeNavigation)
                    .WithMany(p => p.Parcel)
                    .HasForeignKey(d => d.RoeStatusCode)
                    .HasConstraintName("FK_ROWM.Parcel_ROWM.Roe_Status_RoeStatusCode");
            });

            modelBuilder.Entity<ParcelContactInfo>(entity =>
            {
                entity.HasKey(e => new { e.ContactInfoContactId, e.ParcelParcelId })
                    .HasName("PK_ROWM.ParcelContactInfo");

                entity.HasIndex(e => e.ContactInfoContactId)
                    .HasName("IX_ContactInfo_ContactId");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.HasOne(d => d.ContactInfoContact)
                    .WithMany(p => p.ParcelContactInfo)
                    .HasForeignKey(d => d.ContactInfoContactId)
                    .HasConstraintName("FK_dbo.ParcelContactInfoes_ROWM.ContactInfo_ContactInfo_ContactId");

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.ParcelContactInfo)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_dbo.ParcelContactInfoes_ROWM.Parcel_Parcel_ParcelId");
            });

            modelBuilder.Entity<ParcelContactLogs>(entity =>
            {
                entity.HasKey(e => new { e.ContactLogContactLogId, e.ParcelParcelId })
                    .HasName("PK_ROWM.ParcelContactLogs");

                entity.HasIndex(e => e.ContactLogContactLogId)
                    .HasName("IX_ContactLog_ContactLogId");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.HasOne(d => d.ContactLogContactLog)
                    .WithMany(p => p.ParcelContactLogs)
                    .HasForeignKey(d => d.ContactLogContactLogId)
                    .HasConstraintName("FK_ROWM.ParcelContactLogs_ROWM.ContactLog_ContactLog_ContactLogId");

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.ParcelContactLogs)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_ROWM.ParcelContactLogs_ROWM.Parcel_Parcel_ParcelId");
            });

            modelBuilder.Entity<ParcelDocuments>(entity =>
            {
                entity.HasKey(e => new { e.DocumentDocumentId, e.ParcelParcelId })
                    .HasName("PK_ROWM.ParcelDocuments");

                entity.HasIndex(e => e.DocumentDocumentId)
                    .HasName("IX_Document_DocumentId");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.HasOne(d => d.DocumentDocument)
                    .WithMany(p => p.ParcelDocuments)
                    .HasForeignKey(d => d.DocumentDocumentId)
                    .HasConstraintName("FK_ROWM.ParcelDocuments_ROWM.Document_Document_DocumentId");

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.ParcelDocuments)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_ROWM.ParcelDocuments_ROWM.Parcel_Parcel_ParcelId");
            });

            modelBuilder.Entity<ParcelStatus>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_ROWM.Parcel_Status");

                entity.HasOne(d => d.ParentStatusCodeNavigation)
                    .WithMany(p => p.InverseParentStatusCodeNavigation)
                    .HasForeignKey(d => d.ParentStatusCode)
                    .HasConstraintName("FK_Parcel_Status_Parent_Milestone");
            });

            modelBuilder.Entity<PreferredContactMode>(entity =>
            {
                entity.HasKey(e => e.Mode)
                    .HasName("PK__Preferre__2488AC273F1BD714");
            });

            modelBuilder.Entity<RepresentationType>(entity =>
            {
                entity.HasKey(e => e.RelationTypeCode)
                    .HasName("PK_ROWM.Repesentation_Type");
            });

            modelBuilder.Entity<RoeConditions>(entity =>
            {
                entity.HasKey(e => e.ConditionId)
                    .HasName("PK_dbo.RoeConditions");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.Property(e => e.ConditionId).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.ContactNavigation)
                    .WithMany(p => p.RoeConditions)
                    .HasForeignKey(d => d.Contact)
                    .HasConstraintName("FK_CONDITIONS_CONTACTINFO");

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.RoeConditions)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_dbo.RoeConditions_ROWM.Parcel_Parcel_ParcelId");
            });

            modelBuilder.Entity<RoeStatus>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_ROWM.Roe_Status");
            });

            modelBuilder.Entity<StatusActivity>(entity =>
            {
                entity.HasKey(e => e.ActivityId)
                    .HasName("PK_ROWM.StatusActivity");

                entity.HasIndex(e => e.AgentId)
                    .HasName("IX_AgentId");

                entity.HasIndex(e => e.ParentParcelId)
                    .HasName("IX_ParentParcelId");

                entity.Property(e => e.ActivityId).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.StatusActivity)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_ROWM.StatusActivity_ROWM.Agent_AgentId");

                entity.HasOne(d => d.ParentParcel)
                    .WithMany(p => p.StatusActivity)
                    .HasForeignKey(d => d.ParentParcelId)
                    .HasConstraintName("FK_ROWM.StatusActivity_ROWM.Parcel_ParentParcelId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
