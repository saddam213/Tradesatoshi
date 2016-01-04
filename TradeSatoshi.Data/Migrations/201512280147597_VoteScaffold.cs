namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoteScaffold : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoteItemId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Type = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
				.ForeignKey("dbo.VoteItem", t => t.VoteItemId, cascadeDelete: false)
                .Index(t => t.VoteItemId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.VoteItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 128),
                        AdminNote = c.String(maxLength: 265),
                        Status = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vote", "VoteItemId", "dbo.VoteItem");
            DropForeignKey("dbo.VoteItem", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Vote", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.VoteItem", new[] { "UserId" });
            DropIndex("dbo.Vote", new[] { "UserId" });
            DropIndex("dbo.Vote", new[] { "VoteItemId" });
            DropTable("dbo.VoteItem");
            DropTable("dbo.Vote");
        }
    }
}
