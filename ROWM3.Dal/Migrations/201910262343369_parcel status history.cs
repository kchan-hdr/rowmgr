namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class parcelstatushistory : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "dbo.RoeConditions", newSchema: "ROWM");
            CreateTable(
                "ROWM.StatusActivity",
                c => new
                    {
                        ActivityId = c.Guid(nullable: false, identity: true),
                        ActivityDate = c.DateTimeOffset(nullable: false, precision: 7),
                        ParentParcelId = c.Guid(nullable: false),
                        AgentId = c.Guid(nullable: false),
                        ParcelStatusCode = c.String(maxLength: 40),
                        OrigianlParcelStatusCode = c.String(maxLength: 40),
                        RoeStatusCode = c.String(maxLength: 40),
                        OriginalRoeStatusCode = c.String(maxLength: 40),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("ROWM.Agent", t => t.AgentId, cascadeDelete: true)
                .ForeignKey("ROWM.Parcel", t => t.ParentParcelId, cascadeDelete: true)
                .Index(t => t.ParentParcelId)
                .Index(t => t.AgentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("ROWM.StatusActivity", "ParentParcelId", "ROWM.Parcel");
            DropForeignKey("ROWM.StatusActivity", "AgentId", "ROWM.Agent");
            DropIndex("ROWM.StatusActivity", new[] { "AgentId" });
            DropIndex("ROWM.StatusActivity", new[] { "ParentParcelId" });
            DropTable("ROWM.StatusActivity");
            MoveTable(name: "ROWM.RoeConditions", newSchema: "dbo");
        }
    }
}
