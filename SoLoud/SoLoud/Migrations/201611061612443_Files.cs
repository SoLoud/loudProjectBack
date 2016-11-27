namespace SoLoud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Files : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.File",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        ContentType = c.String(maxLength: 100),
                        Content = c.Binary(),
                        FileType = c.Int(nullable: false),
                        ItemId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.FileId)
                .ForeignKey("dbo.ContentItem", t => t.ItemId)
                .Index(t => t.ItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.File", "ItemId", "dbo.ContentItem");
            DropIndex("dbo.File", new[] { "ItemId" });
            DropTable("dbo.File");
        }
    }
}
