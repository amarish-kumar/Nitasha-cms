using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace NITASA.Data
{
    public class NTSDBContext : DbContext,IDbContext
    {
        public NTSDBContext(string ConnectionName = "NITASAConnection")
            : base(ConnectionName)
        {
            
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RightsInRole> RightsInRole { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<ContentLabel> ContentLabel { get; set; }
        public DbSet<ContentCategory> ContentCategory { get; set; }
        public DbSet<ContentView> ContentView { get; set; }
        public DbSet<Label> Label { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Meta> Meta { get; set; }
        public DbSet<Widget> Widget { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Slider> Sliders{ get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
        
        public DbSet<Comment> Comment { get; set; }
        //public DbSet<BackupHistory> BackupHistory { get; set; }
    }
}