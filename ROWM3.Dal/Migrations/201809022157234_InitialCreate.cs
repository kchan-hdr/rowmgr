namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ROWM.Agent",
                c => new
                    {
                        AgentId = c.Guid(nullable: false),
                        AgentName = c.String(nullable: false, maxLength: 20),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AgentId);
            
            CreateTable(
                "ROWM.ContactLog",
                c => new
                    {
                        ContactLogId = c.Guid(nullable: false),
                        DateAdded = c.DateTimeOffset(nullable: false, precision: 7),
                        ContactAgentId = c.Guid(nullable: false),
                        ContactChannel = c.String(maxLength: 20),
                        ProjectPhase = c.String(maxLength: 20),
                        Title = c.String(maxLength: 200),
                        Notes = c.String(),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                        Owner_OwnerId = c.Guid(),
                        Landowner_Score = c.Int(),
                    })
                .PrimaryKey(t => t.ContactLogId)
                .ForeignKey("ROWM.Owner", t => t.Owner_OwnerId)
                .ForeignKey("ROWM.Agent", t => t.ContactAgentId, cascadeDelete: true)
                .Index(t => t.ContactAgentId)
                .Index(t => t.Owner_OwnerId);
            
            CreateTable(
                "ROWM.ContactInfo",
                c => new
                    {
                        ContactId = c.Guid(nullable: false),
                        IsPrimaryContact = c.Boolean(nullable: false),
                        OwnerFirstName = c.String(maxLength: 50),
                        OwnerLastName = c.String(maxLength: 50),
                        OwnerStreetAddress = c.String(maxLength: 400),
                        OwnerCity = c.String(maxLength: 100),
                        OwnerState = c.String(maxLength: 20),
                        OwnerZIP = c.String(maxLength: 10),
                        OwnerEmail = c.String(maxLength: 256),
                        OwnerHomePhone = c.String(),
                        OwnerCellPhone = c.String(),
                        OwnerWorkPhone = c.String(),
                        ContactOwnerId = c.Guid(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                        Representation = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.ContactId)
                .ForeignKey("ROWM.Owner", t => t.ContactOwnerId, cascadeDelete: true)
                .Index(t => t.ContactOwnerId);
            
            CreateTable(
                "ROWM.Owner",
                c => new
                    {
                        OwnerId = c.Guid(nullable: false),
                        PartyName = c.String(maxLength: 200),
                        OwnerType = c.String(maxLength: 50),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.OwnerId);
            
            CreateTable(
                "ROWM.Document",
                c => new
                    {
                        DocumentId = c.Guid(nullable: false),
                        DocumentType = c.String(maxLength: 50),
                        Content = c.Binary(),
                        SharePointUrl = c.String(),
                        AzureBlobId = c.String(),
                        TitleInFile = c.Boolean(nullable: false),
                        ReceivedDate = c.DateTimeOffset(precision: 7),
                        QCDate = c.DateTimeOffset(precision: 7),
                        ApprovedDate = c.DateTimeOffset(precision: 7),
                        SentDate = c.DateTimeOffset(precision: 7),
                        RowmTrackingNumber = c.String(maxLength: 100),
                        DeliveredDate = c.DateTimeOffset(precision: 7),
                        SignedDate = c.DateTimeOffset(precision: 7),
                        ReceivedFromOwnerDate = c.DateTimeOffset(precision: 7),
                        ClientTrackingNumber = c.String(maxLength: 100),
                        ClientSignatureDate = c.DateTimeOffset(precision: 7),
                        ReceivedFromClientDate = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                        DocumentPackage_PackageId = c.Guid(),
                        Title = c.String(),
                        ContentType = c.String(maxLength: 100),
                        SourceFilename = c.String(maxLength: 500),
                        DateRecorded = c.DateTimeOffset(precision: 7),
                        CheckNo = c.String(),
                    })
                .PrimaryKey(t => t.DocumentId)
                .ForeignKey("ROWM.DocumentPackage", t => t.DocumentPackage_PackageId)
                .Index(t => t.DocumentPackage_PackageId);
            
            CreateTable(
                "ROWM.DocumentActivity",
                c => new
                    {
                        ActivityId = c.Guid(nullable: false),
                        ActivityDate = c.DateTimeOffset(nullable: false, precision: 7),
                        Activity = c.String(nullable: false, maxLength: 100),
                        ActivityNotes = c.String(),
                        ParentDocumentId = c.Guid(nullable: false),
                        ChildDocumentId = c.Guid(),
                        AgentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("ROWM.Agent", t => t.AgentId, cascadeDelete: true)
                .ForeignKey("ROWM.Document", t => t.ChildDocumentId)
                .ForeignKey("ROWM.Document", t => t.ParentDocumentId, cascadeDelete: true)
                .Index(t => t.ParentDocumentId)
                .Index(t => t.ChildDocumentId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "ROWM.DocumentPackage",
                c => new
                    {
                        PackageId = c.Guid(nullable: false),
                        PackageName = c.String(nullable: false, maxLength: 100),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.PackageId);
            
            CreateTable(
                "ROWM.Parcel",
                c => new
                    {
                        ParcelId = c.Guid(nullable: false),
                        Assessor_Parcel_Number = c.String(nullable: false, maxLength: 128),
                        County_FIPS = c.String(nullable: false, maxLength: 5, fixedLength: true, unicode: false),
                        County_Name = c.String(maxLength: 50),
                        SitusAddress = c.String(maxLength: 800),
                        Acreage = c.Double(),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                        InitialROEOffer_OfferDate = c.DateTimeOffset(precision: 7),
                        InitialROEOffer_OfferAmount = c.Double(),
                        InitialROEOffer_OfferNotes = c.String(),
                        FinalROEOffer_OfferDate = c.DateTimeOffset(precision: 7),
                        FinalROEOffer_OfferAmount = c.Double(),
                        FinalROEOffer_OfferNotes = c.String(),
                        InitialOptionOffer_OfferDate = c.DateTimeOffset(precision: 7),
                        InitialOptionOffer_OfferAmount = c.Double(),
                        InitialOptionOffer_OfferNotes = c.String(),
                        FinalOptionOffer_OfferDate = c.DateTimeOffset(precision: 7),
                        FinalOptionOffer_OfferAmount = c.Double(),
                        FinalOptionOffer_OfferNotes = c.String(),
                        InitialEasementOffer_OfferDate = c.DateTimeOffset(precision: 7),
                        InitialEasementOffer_OfferAmount = c.Double(),
                        InitialEasementOffer_OfferNotes = c.String(),
                        FinalEasementOffer_OfferDate = c.DateTimeOffset(precision: 7),
                        FinalEasementOffer_OfferAmount = c.Double(),
                        FinalEasementOffer_OfferNotes = c.String(),
                        ParcelStatusCode = c.String(maxLength: 40),
                        RoeStatusCode = c.String(maxLength: 40),
                        IsActive = c.Boolean(nullable: false),
                        Landowner_Score = c.Int(),
                    })
                .PrimaryKey(t => t.ParcelId)
                .ForeignKey("ROWM.Parcel_Status", t => t.ParcelStatusCode)
                .ForeignKey("ROWM.Roe_Status", t => t.RoeStatusCode)
                .Index(t => t.ParcelStatusCode)
                .Index(t => t.RoeStatusCode);
            
            CreateTable(
                "ROWM.Ownership",
                c => new
                    {
                        OwnershipId = c.Guid(nullable: false),
                        ParcelId = c.Guid(),
                        OwnerId = c.Guid(nullable: false),
                        Ownership_t = c.Int(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.OwnershipId)
                .ForeignKey("ROWM.Owner", t => t.OwnerId, cascadeDelete: true)
                .ForeignKey("ROWM.Parcel", t => t.ParcelId)
                .Index(t => t.ParcelId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "ROWM.Parcel_Status",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 40),
                        DomainValue = c.Int(nullable: false),
                        Description = c.String(maxLength: 200),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "ROWM.Roe_Status",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 40),
                        DomainValue = c.Int(nullable: false),
                        Description = c.String(maxLength: 200),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "ROWM.Contact_Channel",
                c => new
                    {
                        ContactTypeCode = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 20),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ContactTypeCode);
            
            CreateTable(
                "ROWM.Contact_Purpose",
                c => new
                    {
                        PurposeCode = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 20),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PurposeCode);
            
            CreateTable(
                "ROWM.Document_Type",
                c => new
                    {
                        DocTypeName = c.String(nullable: false, maxLength: 200),
                        Description = c.String(),
                        FolderPath = c.String(maxLength: 400),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DocTypeName);
            
            CreateTable(
                "ROWM.Landowner_Score",
                c => new
                    {
                        Score = c.Int(nullable: false),
                        DomainValue = c.Int(),
                        Caption = c.String(maxLength: 50),
                        DisplayOrder = c.Int(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.Score);
            
            CreateTable(
                "ROWM.Repesentation_Type",
                c => new
                    {
                        RelationTypeCode = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 20),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RelationTypeCode);
            
            CreateTable(
                "ROWM.ContactInfoContactLogs",
                c => new
                    {
                        ContactInfo_ContactId = c.Guid(nullable: false),
                        ContactLog_ContactLogId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContactInfo_ContactId, t.ContactLog_ContactLogId })
                .ForeignKey("ROWM.ContactInfo", t => t.ContactInfo_ContactId, cascadeDelete: true)
                .ForeignKey("ROWM.ContactLog", t => t.ContactLog_ContactLogId, cascadeDelete: true)
                .Index(t => t.ContactInfo_ContactId)
                .Index(t => t.ContactLog_ContactLogId);
            
            CreateTable(
                "ROWM.OwnerDocuments",
                c => new
                    {
                        Document_DocumentId = c.Guid(nullable: false),
                        Owner_OwnerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Document_DocumentId, t.Owner_OwnerId })
                .ForeignKey("ROWM.Document", t => t.Document_DocumentId, cascadeDelete: true)
                .ForeignKey("ROWM.Owner", t => t.Owner_OwnerId, cascadeDelete: true)
                .Index(t => t.Document_DocumentId)
                .Index(t => t.Owner_OwnerId);
            
            CreateTable(
                "ROWM.ParcelDocuments",
                c => new
                    {
                        Document_DocumentId = c.Guid(nullable: false),
                        Parcel_ParcelId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Document_DocumentId, t.Parcel_ParcelId })
                .ForeignKey("ROWM.Document", t => t.Document_DocumentId, cascadeDelete: true)
                .ForeignKey("ROWM.Parcel", t => t.Parcel_ParcelId, cascadeDelete: true)
                .Index(t => t.Document_DocumentId)
                .Index(t => t.Parcel_ParcelId);
            
            CreateTable(
                "ROWM.ParcelContactLogs",
                c => new
                    {
                        ContactLog_ContactLogId = c.Guid(nullable: false),
                        Parcel_ParcelId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContactLog_ContactLogId, t.Parcel_ParcelId })
                .ForeignKey("ROWM.ContactLog", t => t.ContactLog_ContactLogId, cascadeDelete: true)
                .ForeignKey("ROWM.Parcel", t => t.Parcel_ParcelId, cascadeDelete: true)
                .Index(t => t.ContactLog_ContactLogId)
                .Index(t => t.Parcel_ParcelId);
            
            CreateTable(
                "ROWM.DocumentAgents",
                c => new
                    {
                        Agent_AgentId = c.Guid(nullable: false),
                        Document_DocumentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Agent_AgentId, t.Document_DocumentId })
                .ForeignKey("ROWM.Agent", t => t.Agent_AgentId, cascadeDelete: true)
                .ForeignKey("ROWM.Document", t => t.Document_DocumentId, cascadeDelete: true)
                .Index(t => t.Agent_AgentId)
                .Index(t => t.Document_DocumentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("ROWM.DocumentAgents", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentAgents", "Agent_AgentId", "ROWM.Agent");
            DropForeignKey("ROWM.ContactLog", "ContactAgentId", "ROWM.Agent");
            DropForeignKey("ROWM.ParcelContactLogs", "Parcel_ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.ParcelContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.ParcelDocuments", "Parcel_ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.ParcelDocuments", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.Parcel", "RoeStatusCode", "ROWM.Roe_Status");
            DropForeignKey("ROWM.Parcel", "ParcelStatusCode", "ROWM.Parcel_Status");
            DropForeignKey("ROWM.Ownership", "ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.Ownership", "OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.OwnerDocuments", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.OwnerDocuments", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.Document", "DocumentPackage_PackageId", "ROWM.DocumentPackage");
            DropForeignKey("ROWM.DocumentActivity", "ParentDocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentActivity", "ChildDocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentActivity", "AgentId", "ROWM.Agent");
            DropForeignKey("ROWM.ContactLog", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.ContactInfo", "ContactOwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.ContactInfoContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.ContactInfoContactLogs", "ContactInfo_ContactId", "ROWM.ContactInfo");
            DropIndex("ROWM.DocumentAgents", new[] { "Document_DocumentId" });
            DropIndex("ROWM.DocumentAgents", new[] { "Agent_AgentId" });
            DropIndex("ROWM.ParcelContactLogs", new[] { "Parcel_ParcelId" });
            DropIndex("ROWM.ParcelContactLogs", new[] { "ContactLog_ContactLogId" });
            DropIndex("ROWM.ParcelDocuments", new[] { "Parcel_ParcelId" });
            DropIndex("ROWM.ParcelDocuments", new[] { "Document_DocumentId" });
            DropIndex("ROWM.OwnerDocuments", new[] { "Owner_OwnerId" });
            DropIndex("ROWM.OwnerDocuments", new[] { "Document_DocumentId" });
            DropIndex("ROWM.ContactInfoContactLogs", new[] { "ContactLog_ContactLogId" });
            DropIndex("ROWM.ContactInfoContactLogs", new[] { "ContactInfo_ContactId" });
            DropIndex("ROWM.Ownership", new[] { "OwnerId" });
            DropIndex("ROWM.Ownership", new[] { "ParcelId" });
            DropIndex("ROWM.Parcel", new[] { "RoeStatusCode" });
            DropIndex("ROWM.Parcel", new[] { "ParcelStatusCode" });
            DropIndex("ROWM.DocumentActivity", new[] { "AgentId" });
            DropIndex("ROWM.DocumentActivity", new[] { "ChildDocumentId" });
            DropIndex("ROWM.DocumentActivity", new[] { "ParentDocumentId" });
            DropIndex("ROWM.Document", new[] { "DocumentPackage_PackageId" });
            DropIndex("ROWM.ContactInfo", new[] { "ContactOwnerId" });
            DropIndex("ROWM.ContactLog", new[] { "Owner_OwnerId" });
            DropIndex("ROWM.ContactLog", new[] { "ContactAgentId" });
            DropTable("ROWM.DocumentAgents");
            DropTable("ROWM.ParcelContactLogs");
            DropTable("ROWM.ParcelDocuments");
            DropTable("ROWM.OwnerDocuments");
            DropTable("ROWM.ContactInfoContactLogs");
            DropTable("ROWM.Repesentation_Type");
            DropTable("ROWM.Landowner_Score");
            DropTable("ROWM.Document_Type");
            DropTable("ROWM.Contact_Purpose");
            DropTable("ROWM.Contact_Channel");
            DropTable("ROWM.Roe_Status");
            DropTable("ROWM.Parcel_Status");
            DropTable("ROWM.Ownership");
            DropTable("ROWM.Parcel");
            DropTable("ROWM.DocumentPackage");
            DropTable("ROWM.DocumentActivity");
            DropTable("ROWM.Document");
            DropTable("ROWM.Owner");
            DropTable("ROWM.ContactInfo");
            DropTable("ROWM.ContactLog");
            DropTable("ROWM.Agent");
        }
    }
}
