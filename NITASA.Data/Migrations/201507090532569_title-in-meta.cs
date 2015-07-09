namespace NITASA.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class titleinmeta : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metas", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Metas", "Title");
        }
    }
}
