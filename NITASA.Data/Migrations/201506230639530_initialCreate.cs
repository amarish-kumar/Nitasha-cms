namespace NITASA.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityLogs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        AddedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        Email = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 100),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        ProfilePicURL = c.String(),
                        SaltKey = c.Int(nullable: false),
                        AddedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        AddedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.RoleID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        Name = c.String(nullable: false, maxLength: 100),
                        AddedOn = c.DateTime(nullable: false),
                        AddedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        Name = c.String(nullable: false, maxLength: 100),
                        Slug = c.String(),
                        Description = c.String(),
                        ParentCategoryID = c.Int(),
                        AddedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        AddedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ContentID = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        UserName = c.String(),
                        ProfilePicUrl = c.String(),
                        AddedOn = c.DateTime(nullable: false),
                        IsModerated = c.Boolean(nullable: false),
                        ModeratedBy = c.Int(),
                        ModeratedOn = c.DateTime(),
                        IsAbused = c.Boolean(nullable: false),
                        AbusedBy = c.String(),
                        AbusedReason = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Contents", t => t.ContentID, cascadeDelete: true)
                .Index(t => t.ContentID);
            
            CreateTable(
                "dbo.Contents",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        Type = c.String(),
                        Title = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false),
                        URL = c.String(),
                        CoverContent = c.String(),
                        FeaturedImage = c.String(),
                        IsSlugEdited = c.Boolean(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                        ContentOrder = c.Int(nullable: false),
                        EnableComment = c.Boolean(nullable: false),
                        CommentEnabledTill = c.Int(nullable: false),
                        ContentView = c.Int(nullable: false),
                        AddedOn = c.DateTime(nullable: false),
                        isPublished = c.Boolean(nullable: false),
                        PublishedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        AddedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.AddedBy)
                .Index(t => t.AddedBy);
            
            CreateTable(
                "dbo.ContentCategories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CategoryID = c.Int(nullable: false),
                        ContentID = c.Int(nullable: false),
                        AddedOn = c.DateTime(),
                        AddedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Contents", t => t.ContentID, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.ContentID);
            
            CreateTable(
                "dbo.ContentLabels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LabelID = c.Int(nullable: false),
                        ContentID = c.Int(nullable: false),
                        AddedOn = c.DateTime(),
                        AddedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Contents", t => t.ContentID, cascadeDelete: true)
                .ForeignKey("dbo.Labels", t => t.LabelID, cascadeDelete: true)
                .Index(t => t.LabelID)
                .Index(t => t.ContentID);
            
            CreateTable(
                "dbo.Labels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        Name = c.String(nullable: false, maxLength: 100),
                        Slug = c.String(),
                        Description = c.String(),
                        AddedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        AddedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ContentViews",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ContentID = c.Int(nullable: false),
                        ViewedOn = c.DateTime(),
                        IPAddress = c.String(),
                        Browser = c.String(),
                        BVersion = c.String(),
                        Resolution = c.String(),
                        OSName = c.String(),
                        OSVersion = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        Name = c.String(maxLength: 100),
                        Path = c.String(nullable: false),
                        Type = c.String(),
                        Attribute = c.String(),
                        Extention = c.String(),
                        AddedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        AddedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        ParentMenuID = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 40),
                        Type = c.String(),
                        RefID = c.Int(nullable: false),
                        Slug = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        AddedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        AddedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Metas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ContentID = c.Int(nullable: false),
                        Keyword = c.String(),
                        Description = c.String(),
                        Author = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.RightsInRoles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleID = c.Int(nullable: false),
                        RightsName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Sliders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        Name = c.String(nullable: false),
                        Code = c.String(),
                        AddedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        AddedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Slides",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GUID = c.String(),
                        SliderId = c.Int(nullable: false),
                        Image = c.String(nullable: false),
                        Title = c.String(),
                        Text = c.String(),
                        Link = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        AddedOn = c.DateTime(nullable: false),
                        AddedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Sliders", t => t.SliderId, cascadeDelete: true)
                .Index(t => t.SliderId);
            
            CreateTable(
                "dbo.Widgets",
                c => new
                    {
                        WidgetID = c.Int(nullable: false, identity: true),
                        WidgetGUID = c.String(),
                        WidgetName = c.String(),
                        WidgetTitle = c.String(),
                        WidgetOption = c.String(),
                        WidgetOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ActiveBy = c.Int(),
                        ActiveOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.WidgetID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Slides", "SliderId", "dbo.Sliders");
            DropForeignKey("dbo.Comments", "ContentID", "dbo.Contents");
            DropForeignKey("dbo.Contents", "AddedBy", "dbo.Users");
            DropForeignKey("dbo.ContentLabels", "LabelID", "dbo.Labels");
            DropForeignKey("dbo.ContentLabels", "ContentID", "dbo.Contents");
            DropForeignKey("dbo.ContentCategories", "ContentID", "dbo.Contents");
            DropForeignKey("dbo.ContentCategories", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.ActivityLogs", "UserID", "dbo.Users");
            DropForeignKey("dbo.Users", "RoleID", "dbo.Roles");
            DropIndex("dbo.Slides", new[] { "SliderId" });
            DropIndex("dbo.ContentLabels", new[] { "ContentID" });
            DropIndex("dbo.ContentLabels", new[] { "LabelID" });
            DropIndex("dbo.ContentCategories", new[] { "ContentID" });
            DropIndex("dbo.ContentCategories", new[] { "CategoryID" });
            DropIndex("dbo.Contents", new[] { "AddedBy" });
            DropIndex("dbo.Comments", new[] { "ContentID" });
            DropIndex("dbo.Users", new[] { "RoleID" });
            DropIndex("dbo.ActivityLogs", new[] { "UserID" });
            DropTable("dbo.Widgets");
            DropTable("dbo.Slides");
            DropTable("dbo.Sliders");
            DropTable("dbo.Settings");
            DropTable("dbo.RightsInRoles");
            DropTable("dbo.Metas");
            DropTable("dbo.Menus");
            DropTable("dbo.Media");
            DropTable("dbo.ContentViews");
            DropTable("dbo.Labels");
            DropTable("dbo.ContentLabels");
            DropTable("dbo.ContentCategories");
            DropTable("dbo.Contents");
            DropTable("dbo.Comments");
            DropTable("dbo.Categories");
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
            DropTable("dbo.ActivityLogs");
        }
    }
}
