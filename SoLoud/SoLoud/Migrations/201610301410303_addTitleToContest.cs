namespace SoLoud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTitleToContest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContentItem", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContentItem", "Title");
        }
    }
}
