namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Chat : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatMessage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Message = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            AddColumn("dbo.AspNetUsers", "ChatIcon", c => c.String());
            AddColumn("dbo.AspNetUsers", "ChatBanEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatMessage", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ChatMessage", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "ChatBanEnd");
            DropColumn("dbo.AspNetUsers", "ChatIcon");
            DropTable("dbo.ChatMessage");
        }
    }
}
