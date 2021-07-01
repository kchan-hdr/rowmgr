namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class keepowneraddressfromtitle : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ParcelContactInfoes",
                c => new
                    {
                        Parcel_ParcelId = c.Guid(nullable: false),
                        ContactInfo_ContactId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Parcel_ParcelId, t.ContactInfo_ContactId })
                .ForeignKey("ROWM.Parcel", t => t.Parcel_ParcelId, cascadeDelete: true)
                .ForeignKey("ROWM.ContactInfo", t => t.ContactInfo_ContactId, cascadeDelete: true)
                .Index(t => t.Parcel_ParcelId)
                .Index(t => t.ContactInfo_ContactId);
            
            AddColumn("ROWM.Owner", "OwnerAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParcelContactInfoes", "ContactInfo_ContactId", "ROWM.ContactInfo");
            DropForeignKey("dbo.ParcelContactInfoes", "Parcel_ParcelId", "ROWM.Parcel");
            DropIndex("dbo.ParcelContactInfoes", new[] { "ContactInfo_ContactId" });
            DropIndex("dbo.ParcelContactInfoes", new[] { "Parcel_ParcelId" });
            DropColumn("ROWM.Owner", "OwnerAddress");
            DropTable("dbo.ParcelContactInfoes");
        }
    }
}
