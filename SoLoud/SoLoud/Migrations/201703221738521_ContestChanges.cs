namespace SoLoud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContestChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contest", "Title", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contest", "Title", c => c.String());
        }
    }
}
