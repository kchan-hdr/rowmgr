namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixjointable : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ParcelContactInfoes");
            RenameTable(name: "dbo.ParcelContactInfoes", newName: "ParcelContactInfo");
            MoveTable(name: "dbo.ParcelContactInfo", newSchema: "ROWM");
            AddPrimaryKey("ROWM.ParcelContactInfo", new[] { "ContactInfo_ContactId", "Parcel_ParcelId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("ROWM.ParcelContactInfo");
            AddPrimaryKey("ROWM.ParcelContactInfo", new[] { "Parcel_ParcelId", "ContactInfo_ContactId" });
            MoveTable(name: "ROWM.ParcelContactInfo", newSchema: "dbo");
            RenameTable(name: "dbo.ParcelContactInfo", newName: "ParcelContactInfoes");
        }
    }
}
