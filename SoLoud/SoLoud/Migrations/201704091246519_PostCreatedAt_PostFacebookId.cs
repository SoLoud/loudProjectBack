namespace SoLoud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostCreatedAt_PostFacebookId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Post", "CreatedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.Post", "FacebookId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Post", "FacebookId");
            DropColumn("dbo.Post", "CreatedAt");
        }
    }
}
