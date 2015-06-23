namespace NITASA.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Activitylogchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityLogs", "ActionType", c => c.String());
            AddColumn("dbo.ActivityLogs", "PageUrl", c => c.String());
            AddColumn("dbo.ActivityLogs", "RemoteAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityLogs", "RemoteAddress");
            DropColumn("dbo.ActivityLogs", "PageUrl");
            DropColumn("dbo.ActivityLogs", "ActionType");
        }
    }
}
