namespace NITASA.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addonlayoutadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contents", "AddonMasterLayout", c => c.String());
            AddColumn("dbo.Contents", "AddonSubLayout", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contents", "AddonSubLayout");
            DropColumn("dbo.Contents", "AddonMasterLayout");
        }
    }
}
