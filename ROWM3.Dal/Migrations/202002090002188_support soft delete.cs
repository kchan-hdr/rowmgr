namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supportsoftdelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("ROWM.ContactLog", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("ROWM.Document", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("ROWM.Owner", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("ROWM.ContactInfo", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("ROWM.Parcel", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ROWM.Parcel", "IsDeleted");
            DropColumn("ROWM.ContactInfo", "IsDeleted");
            DropColumn("ROWM.Owner", "IsDeleted");
            DropColumn("ROWM.Document", "IsDeleted");
            DropColumn("ROWM.ContactLog", "IsDeleted");
        }
    }
}
