namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class roecondition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoeConditions",
                c => new
                    {
                        ConditionId = c.Guid(nullable: false, identity: true),
                        Condition = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        EffectiveStartDate = c.DateTimeOffset(precision: 7),
                        EffectiveEndDate = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        LastModified = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedBy = c.String(maxLength: 50),
                        Parcel_ParcelId = c.Guid(),
                    })
                .PrimaryKey(t => t.ConditionId)
                .ForeignKey("ROWM.Parcel", t => t.Parcel_ParcelId)
                .Index(t => t.Parcel_ParcelId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoeConditions", "Parcel_ParcelId", "ROWM.Parcel");
            DropIndex("dbo.RoeConditions", new[] { "Parcel_ParcelId" });
            DropTable("dbo.RoeConditions");
        }
    }
}
