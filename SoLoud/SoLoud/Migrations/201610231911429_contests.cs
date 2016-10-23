namespace SoLoud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contest",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        EndingAt = c.DateTimeOffset(nullable: false, precision: 7),
                        Category = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contest", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Contest", new[] { "UserId" });
            DropTable("dbo.Contest");
        }
    }
}
