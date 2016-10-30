namespace SoLoud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeCategoriesToEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContentItem", "Category", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ContentItem", "Category", c => c.String());
        }
    }
}
