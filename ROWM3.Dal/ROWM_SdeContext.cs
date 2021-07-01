namespace ROWM.Dal
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ROWM_SdeContext : ROWM_Context
    {
        /// <summary>
        /// subclass from the original context. 
        /// that db has been made into a SDE. with archiving.
        /// </summary>
        public ROWM_SdeContext()
            : base("name=ROWM_Context")
        {
            Database.SetInitializer<ROWM_SdeContext>(null);
        }

        public ROWM_SdeContext(string c = "name=ROWM_Context")
            : base(c)
        {
            Database.SetInitializer<ROWM_SdeContext>(null);
        }

        //public virtual DbSet<Agent> Agent { get; set; }
        //public virtual DbSet<Contact_Channel> Contact_Channel { get; set; }
        //public virtual DbSet<Contact_Purpose> Contact_Purpose { get; set; }
        //public virtual DbSet<ContactInfo> ContactInfo { get; set; }
        //public virtual DbSet<ContactLog> ContactLog { get; set; }
        //public virtual DbSet<Document> Document { get; set; }
        //public virtual DbSet<Document_Type> Document_Type { get; set; }
        //public virtual DbSet<DocumentActivity> DocumentActivity { get; set; }
        //public virtual DbSet<DocumentPackage> DocumentPackage { get; set; }
        //public virtual DbSet<Landowner_Score> Landowner_Score { get; set; }
        //public virtual DbSet<Owner> Owner { get; set; }
        //public virtual DbSet<Ownership> Ownership { get; set; }
        //public virtual DbSet<Parcel> Parcel { get; set; }
        //public virtual DbSet<Parcel_Status> Parcel_Status { get; set; }
        //public virtual DbSet<Repesentation_Type> Repesentation_Type { get; set; }
        //public virtual DbSet<Roe_Status> Roe_Status { get; set; }
        //public virtual DbSet<RoeCondition> RoeCondition { get; set; }
        //public virtual DbSet<Followup> Followup { get; set; }
        //public virtual DbSet<Organization> Organization { get; set; }
        //public virtual DbSet<MapConfiguration> MapConfiguration { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>().Map(m => m.ToTable("AGENT_EVW", "dbo"));
            modelBuilder.Entity<Contact_Channel>().Map(m => m.ToTable("CONTACT_CHANNEL_EVW", "dbo"));
            modelBuilder.Entity<Contact_Purpose>().Map(m => m.ToTable("CONTACT_PURPOSE_EVW", "dbo"));
            modelBuilder.Entity<ContactInfo>().Map(m => m.ToTable("CONTACTINFO_EVW", "dbo"));
            modelBuilder.Entity<ContactLog>().Map(m => m.ToTable("CONTACTLOG_EVW", "dbo"));
            modelBuilder.Entity<Owner>().Map(m => m.ToTable("OWNER_EVW", "dbo"));
            modelBuilder.Entity<Ownership>().Map(m => m.ToTable("OWNERSHIP_EVW", "dbo"));
            modelBuilder.Entity<Parcel_Status>().Map(m => m.ToTable("PARCEL_STATUS_EVW", "dbo"));
            modelBuilder.Entity<Roe_Status>().Map(m => m.ToTable("ROE_STATUS_EVW", "dbo"));

            modelBuilder.Entity<Parcel>().Map(m => m.ToTable("PARCEL_EVW", "dbo"));
            modelBuilder.Entity<Parcel>().Property(p => p.InitialEasementOffer_OfferAmount).HasColumnName("IEasement_OfferAmount");
            modelBuilder.Entity<Parcel>().Property(p => p.InitialEasementOffer_OfferDate).HasColumnName("IEasement_OfferDate");
            modelBuilder.Entity<Parcel>().Property(p => p.InitialEasementOffer_OfferNotes).HasColumnName("IEasement_OfferNotes");
            modelBuilder.Entity<Parcel>().Property(p => p.InitialOptionOffer_OfferAmount).HasColumnName("IOption_OfferAmount");
            modelBuilder.Entity<Parcel>().Property(p => p.InitialOptionOffer_OfferDate).HasColumnName("IOption_OfferDate");
            modelBuilder.Entity<Parcel>().Property(p => p.InitialOptionOffer_OfferNotes).HasColumnName("IOption_OfferNotes");
            modelBuilder.Entity<Parcel>().Property(p => p.InitialROEOffer_OfferAmount).HasColumnName("IROE_OfferAmount");
            modelBuilder.Entity<Parcel>().Property(p => p.InitialROEOffer_OfferDate).HasColumnName("IROE_OfferDate");
            modelBuilder.Entity<Parcel>().Property(p => p.InitialROEOffer_OfferNotes).HasColumnName("IROE_OfferNotes");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalEasementOffer_OfferAmount).HasColumnName("FEasement_OfferAmount");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalEasementOffer_OfferDate).HasColumnName("FEasement_OfferDate");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalEasementOffer_OfferNotes).HasColumnName("FEasement_OfferNotes");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalOptionOffer_OfferAmount).HasColumnName("FOption_OfferAmount");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalOptionOffer_OfferDate).HasColumnName("FOption_OfferDate");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalOptionOffer_OfferNotes).HasColumnName("FOption_OfferNotes");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalROEOffer_OfferAmount).HasColumnName("FROE_OfferAmount");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalROEOffer_OfferDate).HasColumnName("FROE_OfferDate");
            modelBuilder.Entity<Parcel>().Property(p => p.FinalROEOffer_OfferNotes).HasColumnName("FROE_OfferNotes");




            modelBuilder.Entity<Agent>()
                .HasMany(e => e.ContactLog)
                .WithRequired(e => e.Agent)
                .HasForeignKey(e => e.ContactAgentId);

            modelBuilder.Entity<Agent>()
                .HasMany(e => e.Document)
                .WithMany(e => e.Agent)
                .Map(m => m.ToTable("DocumentAgents", "ROWM"));

            modelBuilder.Entity<ContactInfo>()
                .HasMany(e => e.ContactLog)
                .WithMany(e => e.ContactInfo)
                .Map(m => m.ToTable("ContactInfoContactLogs", "ROWM"));

            modelBuilder.Entity<ContactLog>()
                .HasMany(e => e.Parcel)
                .WithMany(e => e.ContactLog)
                .Map(m => m.ToTable("PARCELCONTACTLOGS_EVW", "dbo"));

            modelBuilder.Entity<Document>()
                .HasMany(e => e.DocumentActivity)
                .WithOptional(e => e.Document)
                .HasForeignKey(e => e.ChildDocumentId);

            modelBuilder.Entity<Document>()
                .HasMany(e => e.DocumentActivity1)
                .WithRequired(e => e.Document1)
                .HasForeignKey(e => e.ParentDocumentId);

            modelBuilder.Entity<Document>()
                .HasMany(e => e.Owner)
                .WithMany(e => e.Document)
                .Map(m => m.ToTable("OwnerDocuments", "ROWM"));

            modelBuilder.Entity<Document>()
                .HasMany(e => e.Parcel)
                .WithMany(e => e.Document)
                .Map(m => m.ToTable("ParcelDocuments", "ROWM"));

            modelBuilder.Entity<DocumentPackage>()
                .HasMany(e => e.Document)
                .WithOptional(e => e.DocumentPackage)
                .HasForeignKey(e => e.DocumentPackage_PackageId);

            modelBuilder.Entity<Owner>()
                .HasMany(e => e.ContactInfo)
                .WithRequired(e => e.Owner)
                .HasForeignKey(e => e.ContactOwnerId);

            modelBuilder.Entity<Owner>()
                .HasMany(e => e.ContactLog)
                .WithOptional(e => e.Owner)
                .HasForeignKey(e => e.Owner_OwnerId);

            modelBuilder.Entity<Parcel>()
                .Property(e => e.County_FIPS)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Parcel_Status>()
                .HasMany(e => e.Parcel)
                .WithOptional(e => e.Parcel_Status)
                .HasForeignKey(e => e.ParcelStatusCode);

            modelBuilder.Entity<Roe_Status>()
                .HasMany(e => e.Parcel)
                .WithOptional(e => e.Roe_Status)
                .HasForeignKey(e => e.RoeStatusCode);

            modelBuilder.Entity<Followup>()
                .HasRequired(f => f.ParentContactLog)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Followup>()
                .HasRequired(f => f.ChildContactLog)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}
