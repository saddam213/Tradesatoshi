namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransferTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransferHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ToUserId = c.String(nullable: false, maxLength: 128),
                        CurrencyId = c.Int(nullable: false),
                        TransferType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Fee = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currency", t => t.CurrencyId, cascadeDelete: false)
				.ForeignKey("dbo.AspNetUsers", t => t.ToUserId, cascadeDelete: false)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.ToUserId)
                .Index(t => t.CurrencyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransferHistory", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TransferHistory", "ToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TransferHistory", "CurrencyId", "dbo.Currency");
            DropIndex("dbo.TransferHistory", new[] { "CurrencyId" });
            DropIndex("dbo.TransferHistory", new[] { "ToUserId" });
            DropIndex("dbo.TransferHistory", new[] { "UserId" });
            DropTable("dbo.TransferHistory");
        }
    }
}
