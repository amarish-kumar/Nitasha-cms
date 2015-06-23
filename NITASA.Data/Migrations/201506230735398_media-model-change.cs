namespace NITASA.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mediamodelchange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "FileName", c => c.String(maxLength: 100));
            DropColumn("dbo.Media", "Name");
            DropColumn("dbo.Media", "Path");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Media", "Path", c => c.String(nullable: false));
            AddColumn("dbo.Media", "Name", c => c.String(maxLength: 100));
            DropColumn("dbo.Media", "FileName");
        }
    }
}
