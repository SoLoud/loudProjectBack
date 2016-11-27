namespace SoLoud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImagesToContests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContentItem", "ProductImageUrl", c => c.String());
            AddColumn("dbo.ContentItem", "ExampleImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContentItem", "ExampleImageUrl");
            DropColumn("dbo.ContentItem", "ProductImageUrl");
        }
    }
}
