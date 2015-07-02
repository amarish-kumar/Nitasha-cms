namespace NITASA.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contentpositioncontentmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contents", "ContentPosition", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contents", "ContentPosition");
        }
    }
}
