namespace NITASA.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commentsmodelchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Website", c => c.String());
            AddColumn("dbo.Comments", "CommentAs", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "CommentAs");
            DropColumn("dbo.Comments", "Website");
        }
    }
}
