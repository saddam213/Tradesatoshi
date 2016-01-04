namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoteSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoteSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Period = c.Int(nullable: false),
                        Next = c.DateTime(nullable: false),
                        LastFreeId = c.Int(),
                        LastPaidId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VoteItem", t => t.LastFreeId)
                .ForeignKey("dbo.VoteItem", t => t.LastPaidId)
                .Index(t => t.LastFreeId)
                .Index(t => t.LastPaidId);
            
            DropTable("dbo.SiteSettings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SiteSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VotePeriod = c.Int(nullable: false),
                        NextVote = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.VoteSettings", "LastPaidId", "dbo.VoteItem");
            DropForeignKey("dbo.VoteSettings", "LastFreeId", "dbo.VoteItem");
            DropIndex("dbo.VoteSettings", new[] { "LastPaidId" });
            DropIndex("dbo.VoteSettings", new[] { "LastFreeId" });
            DropTable("dbo.VoteSettings");
        }
    }
}
