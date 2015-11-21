namespace TradeSatoshi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfileAndSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        BirthDate = c.DateTime(nullable: false),
                        Address = c.String(maxLength: 256),
                        City = c.String(maxLength: 256),
                        State = c.String(maxLength: 256),
                        Country = c.String(maxLength: 256),
                        PostCode = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserSettings",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "Id", "dbo.UserProfile", "Id");
            AddForeignKey("dbo.AspNetUsers", "Id", "dbo.UserSettings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Id", "dbo.UserSettings");
            DropForeignKey("dbo.AspNetUsers", "Id", "dbo.UserProfile");
            DropIndex("dbo.AspNetUsers", new[] { "Id" });
            DropTable("dbo.UserSettings");
            DropTable("dbo.UserProfile");
        }
    }
}
