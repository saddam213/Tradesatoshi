namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SupportTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SupportTicket",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Title = c.String(maxLength: 256),
                        Description = c.String(maxLength: 4000),
                        CategoryId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SupportCategory", t => t.CategoryId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.SupportCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SupportTicketReply",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Message = c.String(maxLength: 4000),
                        IsPublic = c.Boolean(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SupportTicket", t => t.TicketId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.TicketId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SupportFaq",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Question = c.String(maxLength: 256),
                        Answer = c.String(maxLength: 4000),
                        Order = c.Int(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SupportRequest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sender = c.String(),
                        Title = c.String(maxLength: 256),
                        Description = c.String(maxLength: 4000),
                        Created = c.DateTime(nullable: false),
                        Replied = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupportTicket", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SupportTicketReply", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SupportTicketReply", "TicketId", "dbo.SupportTicket");
            DropForeignKey("dbo.SupportTicket", "CategoryId", "dbo.SupportCategory");
            DropIndex("dbo.SupportTicketReply", new[] { "UserId" });
            DropIndex("dbo.SupportTicketReply", new[] { "TicketId" });
            DropIndex("dbo.SupportTicket", new[] { "CategoryId" });
            DropIndex("dbo.SupportTicket", new[] { "UserId" });
            DropTable("dbo.SupportRequest");
            DropTable("dbo.SupportFaq");
            DropTable("dbo.SupportTicketReply");
            DropTable("dbo.SupportCategory");
            DropTable("dbo.SupportTicket");
        }
    }
}
