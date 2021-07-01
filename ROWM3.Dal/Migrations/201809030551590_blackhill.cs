namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blackhill : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("ROWM.ContactInfoContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.ParcelContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.ContactInfoContactLogs", "ContactInfo_ContactId", "ROWM.ContactInfo");
            DropForeignKey("ROWM.ContactInfo", "ContactOwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.ContactLog", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.OwnerDocuments", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.Ownership", "OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.DocumentActivity", "ChildDocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentActivity", "ParentDocumentId", "ROWM.Document");
            DropForeignKey("ROWM.OwnerDocuments", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.ParcelDocuments", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentAgents", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.Document", "DocumentPackage_PackageId", "ROWM.DocumentPackage");
            DropForeignKey("ROWM.Ownership", "ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.ParcelDocuments", "Parcel_ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.ParcelContactLogs", "Parcel_ParcelId", "ROWM.Parcel");
            DropPrimaryKey("ROWM.ContactLog");
            DropPrimaryKey("ROWM.ContactInfo");
            DropPrimaryKey("ROWM.Owner");
            DropPrimaryKey("ROWM.Document");
            DropPrimaryKey("ROWM.DocumentActivity");
            DropPrimaryKey("ROWM.DocumentPackage");
            DropPrimaryKey("ROWM.Parcel");
            DropPrimaryKey("ROWM.Ownership");
            CreateTable(
                "ROWM.Organization",
                c => new
                    {
                        OrganizationId = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        Name = c.String(maxLength: 200),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.OrganizationId);
            
            CreateTable(
                "ROWM.Contact_Followup",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        FollowupType = c.Int(nullable: false),
                        ParentContactLogId = c.Guid(nullable: false),
                        ChildContactLogId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("ROWM.ContactLog", t => t.ChildContactLogId)
                .ForeignKey("ROWM.ContactLog", t => t.ParentContactLogId)
                .Index(t => t.ParentContactLogId)
                .Index(t => t.ChildContactLogId);
            
            CreateTable(
                "App.Map",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                        Caption = c.String(),
                        LayerType = c.Int(nullable: false),
                        AgsUrl = c.String(maxLength: 2048),
                        LayerId = c.String(maxLength: 10),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("ROWM.ContactInfo", "FirstName", c => c.String(maxLength: 50));
            AddColumn("ROWM.ContactInfo", "LastName", c => c.String(maxLength: 50));
            AddColumn("ROWM.ContactInfo", "StreetAddress", c => c.String(maxLength: 400));
            AddColumn("ROWM.ContactInfo", "City", c => c.String(maxLength: 100));
            AddColumn("ROWM.ContactInfo", "State", c => c.String(maxLength: 20));
            AddColumn("ROWM.ContactInfo", "ZIP", c => c.String(maxLength: 10));
            AddColumn("ROWM.ContactInfo", "Email", c => c.String(maxLength: 256));
            AddColumn("ROWM.ContactInfo", "HomePhone", c => c.String());
            AddColumn("ROWM.ContactInfo", "CellPhone", c => c.String());
            AddColumn("ROWM.ContactInfo", "WorkPhone", c => c.String());
            AddColumn("ROWM.ContactInfo", "OrganizationId", c => c.Guid());
            AddColumn("ROWM.Owner", "Owner_OwnerId", c => c.Guid());
            AddColumn("ROWM.Document", "ContactLog_ContactLogId", c => c.Guid());
            AddColumn("ROWM.Parcel", "Tracking_Number", c => c.String(maxLength: 100));
            AddColumn("ROWM.Parcel", "Parcel_ParcelId", c => c.Guid());
            AlterColumn("ROWM.ContactLog", "ContactLogId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("ROWM.ContactInfo", "ContactId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("ROWM.Owner", "OwnerId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("ROWM.Document", "DocumentId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("ROWM.DocumentActivity", "ActivityId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("ROWM.DocumentPackage", "PackageId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("ROWM.Parcel", "ParcelId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("ROWM.Ownership", "OwnershipId", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AddPrimaryKey("ROWM.ContactLog", "ContactLogId");
            AddPrimaryKey("ROWM.ContactInfo", "ContactId");
            AddPrimaryKey("ROWM.Owner", "OwnerId");
            AddPrimaryKey("ROWM.Document", "DocumentId");
            AddPrimaryKey("ROWM.DocumentActivity", "ActivityId");
            AddPrimaryKey("ROWM.DocumentPackage", "PackageId");
            AddPrimaryKey("ROWM.Parcel", "ParcelId");
            AddPrimaryKey("ROWM.Ownership", "OwnershipId");
            CreateIndex("ROWM.Document", "ContactLog_ContactLogId");
            CreateIndex("ROWM.Owner", "Owner_OwnerId");
            CreateIndex("ROWM.ContactInfo", "OrganizationId");
            CreateIndex("ROWM.Parcel", "Parcel_ParcelId");
            AddForeignKey("ROWM.ContactInfo", "OrganizationId", "ROWM.Organization", "OrganizationId");
            AddForeignKey("ROWM.Parcel", "Parcel_ParcelId", "ROWM.Parcel", "ParcelId");
            AddForeignKey("ROWM.Owner", "Owner_OwnerId", "ROWM.Owner", "OwnerId");
            AddForeignKey("ROWM.Document", "ContactLog_ContactLogId", "ROWM.ContactLog", "ContactLogId");
            AddForeignKey("ROWM.ContactInfoContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog", "ContactLogId", cascadeDelete: true);
            AddForeignKey("ROWM.ParcelContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog", "ContactLogId", cascadeDelete: true);
            AddForeignKey("ROWM.ContactInfoContactLogs", "ContactInfo_ContactId", "ROWM.ContactInfo", "ContactId", cascadeDelete: true);
            AddForeignKey("ROWM.ContactInfo", "ContactOwnerId", "ROWM.Owner", "OwnerId", cascadeDelete: true);
            AddForeignKey("ROWM.ContactLog", "Owner_OwnerId", "ROWM.Owner", "OwnerId");
            AddForeignKey("ROWM.Ownership", "OwnerId", "ROWM.Owner", "OwnerId", cascadeDelete: true);
            AddForeignKey("ROWM.OwnerDocuments", "Owner_OwnerId", "ROWM.Owner", "OwnerId", cascadeDelete: true);
            AddForeignKey("ROWM.DocumentActivity", "ChildDocumentId", "ROWM.Document", "DocumentId");
            AddForeignKey("ROWM.DocumentActivity", "ParentDocumentId", "ROWM.Document", "DocumentId", cascadeDelete: true);
            AddForeignKey("ROWM.OwnerDocuments", "Document_DocumentId", "ROWM.Document", "DocumentId", cascadeDelete: true);
            AddForeignKey("ROWM.ParcelDocuments", "Document_DocumentId", "ROWM.Document", "DocumentId", cascadeDelete: true);
            AddForeignKey("ROWM.DocumentAgents", "Document_DocumentId", "ROWM.Document", "DocumentId", cascadeDelete: true);
            AddForeignKey("ROWM.Document", "DocumentPackage_PackageId", "ROWM.DocumentPackage", "PackageId");
            AddForeignKey("ROWM.Ownership", "ParcelId", "ROWM.Parcel", "ParcelId");
            AddForeignKey("ROWM.ParcelDocuments", "Parcel_ParcelId", "ROWM.Parcel", "ParcelId", cascadeDelete: true);
            AddForeignKey("ROWM.ParcelContactLogs", "Parcel_ParcelId", "ROWM.Parcel", "ParcelId", cascadeDelete: true);
            DropColumn("ROWM.ContactInfo", "OwnerFirstName");
            DropColumn("ROWM.ContactInfo", "OwnerLastName");
            DropColumn("ROWM.ContactInfo", "OwnerStreetAddress");
            DropColumn("ROWM.ContactInfo", "OwnerCity");
            DropColumn("ROWM.ContactInfo", "OwnerState");
            DropColumn("ROWM.ContactInfo", "OwnerZIP");
            DropColumn("ROWM.ContactInfo", "OwnerEmail");
            DropColumn("ROWM.ContactInfo", "OwnerHomePhone");
            DropColumn("ROWM.ContactInfo", "OwnerCellPhone");
            DropColumn("ROWM.ContactInfo", "OwnerWorkPhone");
        }
        
        public override void Down()
        {
            AddColumn("ROWM.ContactInfo", "OwnerWorkPhone", c => c.String());
            AddColumn("ROWM.ContactInfo", "OwnerCellPhone", c => c.String());
            AddColumn("ROWM.ContactInfo", "OwnerHomePhone", c => c.String());
            AddColumn("ROWM.ContactInfo", "OwnerEmail", c => c.String(maxLength: 256));
            AddColumn("ROWM.ContactInfo", "OwnerZIP", c => c.String(maxLength: 10));
            AddColumn("ROWM.ContactInfo", "OwnerState", c => c.String(maxLength: 20));
            AddColumn("ROWM.ContactInfo", "OwnerCity", c => c.String(maxLength: 100));
            AddColumn("ROWM.ContactInfo", "OwnerStreetAddress", c => c.String(maxLength: 400));
            AddColumn("ROWM.ContactInfo", "OwnerLastName", c => c.String(maxLength: 50));
            AddColumn("ROWM.ContactInfo", "OwnerFirstName", c => c.String(maxLength: 50));
            DropForeignKey("ROWM.ParcelContactLogs", "Parcel_ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.ParcelDocuments", "Parcel_ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.Ownership", "ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.Document", "DocumentPackage_PackageId", "ROWM.DocumentPackage");
            DropForeignKey("ROWM.DocumentAgents", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.ParcelDocuments", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.OwnerDocuments", "Document_DocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentActivity", "ParentDocumentId", "ROWM.Document");
            DropForeignKey("ROWM.DocumentActivity", "ChildDocumentId", "ROWM.Document");
            DropForeignKey("ROWM.OwnerDocuments", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.Ownership", "OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.ContactLog", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.ContactInfo", "ContactOwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.ContactInfoContactLogs", "ContactInfo_ContactId", "ROWM.ContactInfo");
            DropForeignKey("ROWM.ParcelContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.ContactInfoContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.Contact_Followup", "ParentContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.Contact_Followup", "ChildContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.Document", "ContactLog_ContactLogId", "ROWM.ContactLog");
            DropForeignKey("ROWM.Owner", "Owner_OwnerId", "ROWM.Owner");
            DropForeignKey("ROWM.Parcel", "Parcel_ParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.ContactInfo", "OrganizationId", "ROWM.Organization");
            DropIndex("ROWM.Contact_Followup", new[] { "ChildContactLogId" });
            DropIndex("ROWM.Contact_Followup", new[] { "ParentContactLogId" });
            DropIndex("ROWM.Parcel", new[] { "Parcel_ParcelId" });
            DropIndex("ROWM.ContactInfo", new[] { "OrganizationId" });
            DropIndex("ROWM.Owner", new[] { "Owner_OwnerId" });
            DropIndex("ROWM.Document", new[] { "ContactLog_ContactLogId" });
            DropPrimaryKey("ROWM.Ownership");
            DropPrimaryKey("ROWM.Parcel");
            DropPrimaryKey("ROWM.DocumentPackage");
            DropPrimaryKey("ROWM.DocumentActivity");
            DropPrimaryKey("ROWM.Document");
            DropPrimaryKey("ROWM.Owner");
            DropPrimaryKey("ROWM.ContactInfo");
            DropPrimaryKey("ROWM.ContactLog");
            AlterColumn("ROWM.Ownership", "OwnershipId", c => c.Guid(nullable: false));
            AlterColumn("ROWM.Parcel", "ParcelId", c => c.Guid(nullable: false));
            AlterColumn("ROWM.DocumentPackage", "PackageId", c => c.Guid(nullable: false));
            AlterColumn("ROWM.DocumentActivity", "ActivityId", c => c.Guid(nullable: false));
            AlterColumn("ROWM.Document", "DocumentId", c => c.Guid(nullable: false));
            AlterColumn("ROWM.Owner", "OwnerId", c => c.Guid(nullable: false));
            AlterColumn("ROWM.ContactInfo", "ContactId", c => c.Guid(nullable: false));
            AlterColumn("ROWM.ContactLog", "ContactLogId", c => c.Guid(nullable: false));
            DropColumn("ROWM.Parcel", "Parcel_ParcelId");
            DropColumn("ROWM.Parcel", "Tracking_Number");
            DropColumn("ROWM.Document", "ContactLog_ContactLogId");
            DropColumn("ROWM.Owner", "Owner_OwnerId");
            DropColumn("ROWM.ContactInfo", "OrganizationId");
            DropColumn("ROWM.ContactInfo", "WorkPhone");
            DropColumn("ROWM.ContactInfo", "CellPhone");
            DropColumn("ROWM.ContactInfo", "HomePhone");
            DropColumn("ROWM.ContactInfo", "Email");
            DropColumn("ROWM.ContactInfo", "ZIP");
            DropColumn("ROWM.ContactInfo", "State");
            DropColumn("ROWM.ContactInfo", "City");
            DropColumn("ROWM.ContactInfo", "StreetAddress");
            DropColumn("ROWM.ContactInfo", "LastName");
            DropColumn("ROWM.ContactInfo", "FirstName");
            DropTable("App.Map");
            DropTable("ROWM.Contact_Followup");
            DropTable("ROWM.Organization");
            AddPrimaryKey("ROWM.Ownership", "OwnershipId");
            AddPrimaryKey("ROWM.Parcel", "ParcelId");
            AddPrimaryKey("ROWM.DocumentPackage", "PackageId");
            AddPrimaryKey("ROWM.DocumentActivity", "ActivityId");
            AddPrimaryKey("ROWM.Document", "DocumentId");
            AddPrimaryKey("ROWM.Owner", "OwnerId");
            AddPrimaryKey("ROWM.ContactInfo", "ContactId");
            AddPrimaryKey("ROWM.ContactLog", "ContactLogId");
            AddForeignKey("ROWM.ParcelContactLogs", "Parcel_ParcelId", "ROWM.Parcel", "ParcelId", cascadeDelete: true);
            AddForeignKey("ROWM.ParcelDocuments", "Parcel_ParcelId", "ROWM.Parcel", "ParcelId", cascadeDelete: true);
            AddForeignKey("ROWM.Ownership", "ParcelId", "ROWM.Parcel", "ParcelId");
            AddForeignKey("ROWM.Document", "DocumentPackage_PackageId", "ROWM.DocumentPackage", "PackageId");
            AddForeignKey("ROWM.DocumentAgents", "Document_DocumentId", "ROWM.Document", "DocumentId", cascadeDelete: true);
            AddForeignKey("ROWM.ParcelDocuments", "Document_DocumentId", "ROWM.Document", "DocumentId", cascadeDelete: true);
            AddForeignKey("ROWM.OwnerDocuments", "Document_DocumentId", "ROWM.Document", "DocumentId", cascadeDelete: true);
            AddForeignKey("ROWM.DocumentActivity", "ParentDocumentId", "ROWM.Document", "DocumentId", cascadeDelete: true);
            AddForeignKey("ROWM.DocumentActivity", "ChildDocumentId", "ROWM.Document", "DocumentId");
            AddForeignKey("ROWM.Ownership", "OwnerId", "ROWM.Owner", "OwnerId", cascadeDelete: true);
            AddForeignKey("ROWM.OwnerDocuments", "Owner_OwnerId", "ROWM.Owner", "OwnerId", cascadeDelete: true);
            AddForeignKey("ROWM.ContactLog", "Owner_OwnerId", "ROWM.Owner", "OwnerId");
            AddForeignKey("ROWM.ContactInfo", "ContactOwnerId", "ROWM.Owner", "OwnerId", cascadeDelete: true);
            AddForeignKey("ROWM.ContactInfoContactLogs", "ContactInfo_ContactId", "ROWM.ContactInfo", "ContactId", cascadeDelete: true);
            AddForeignKey("ROWM.ParcelContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog", "ContactLogId", cascadeDelete: true);
            AddForeignKey("ROWM.ContactInfoContactLogs", "ContactLog_ContactLogId", "ROWM.ContactLog", "ContactLogId", cascadeDelete: true);
        }
    }
}
