namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class denverenhancement : DbMigration
    {
        public override void Up()
        {
            AddColumn("ROWM.Contact_Purpose", "MilestoneCode", c => c.String(maxLength: 40));
            AddColumn("ROWM.Document_Type", "MilestoneCode", c => c.String(maxLength: 40));
            CreateIndex("ROWM.Contact_Purpose", "MilestoneCode");
            CreateIndex("ROWM.Document_Type", "MilestoneCode");
            AddForeignKey("ROWM.Contact_Purpose", "MilestoneCode", "ROWM.Parcel_Status", "Code");
            AddForeignKey("ROWM.Document_Type", "MilestoneCode", "ROWM.Parcel_Status", "Code");
        }
        
        public override void Down()
        {
            DropForeignKey("ROWM.Document_Type", "MilestoneCode", "ROWM.Parcel_Status");
            DropForeignKey("ROWM.Contact_Purpose", "MilestoneCode", "ROWM.Parcel_Status");
            DropIndex("ROWM.Document_Type", new[] { "MilestoneCode" });
            DropIndex("ROWM.Contact_Purpose", new[] { "MilestoneCode" });
            DropColumn("ROWM.Document_Type", "MilestoneCode");
            DropColumn("ROWM.Contact_Purpose", "MilestoneCode");
        }
    }
}
