namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TradeTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        TradePairId = c.Int(nullable: false),
                        TradeType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Rate = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Fee = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Timestamp = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Remaining = c.Decimal(nullable: false, precision: 38, scale: 8),
                        IsApi = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
				.ForeignKey("dbo.TradePair", t => t.TradePairId, cascadeDelete: false)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.TradePairId);
            
            CreateTable(
                "dbo.TradePair",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyId1 = c.Int(nullable: false),
                        CurrencyId2 = c.Int(nullable: false),
                        LastTrade = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Change = c.Double(nullable: false),
                        Status = c.Int(nullable: false),
                        StatusMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currency", t => t.CurrencyId1, cascadeDelete: false)
                .ForeignKey("dbo.Currency", t => t.CurrencyId2, cascadeDelete: false)
                .Index(t => t.CurrencyId1)
                .Index(t => t.CurrencyId2);
            
            CreateTable(
                "dbo.TradeHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ToUserId = c.String(nullable: false, maxLength: 128),
                        TradePairId = c.Int(nullable: false),
                        CurrencyId = c.Int(nullable: false),
                        TradeHistoryType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Rate = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Fee = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Timestamp = c.DateTime(nullable: false),
                        IsApi = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
				.ForeignKey("dbo.Currency", t => t.CurrencyId, cascadeDelete: false)
				.ForeignKey("dbo.AspNetUsers", t => t.ToUserId, cascadeDelete: false)
				.ForeignKey("dbo.TradePair", t => t.TradePairId, cascadeDelete: false)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.ToUserId)
                .Index(t => t.TradePairId)
                .Index(t => t.CurrencyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TradeHistory", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TradeHistory", "TradePairId", "dbo.TradePair");
            DropForeignKey("dbo.TradeHistory", "ToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TradeHistory", "CurrencyId", "dbo.Currency");
            DropForeignKey("dbo.Trade", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Trade", "TradePairId", "dbo.TradePair");
            DropForeignKey("dbo.TradePair", "CurrencyId2", "dbo.Currency");
            DropForeignKey("dbo.TradePair", "CurrencyId1", "dbo.Currency");
            DropIndex("dbo.TradeHistory", new[] { "CurrencyId" });
            DropIndex("dbo.TradeHistory", new[] { "TradePairId" });
            DropIndex("dbo.TradeHistory", new[] { "ToUserId" });
            DropIndex("dbo.TradeHistory", new[] { "UserId" });
            DropIndex("dbo.TradePair", new[] { "CurrencyId2" });
            DropIndex("dbo.TradePair", new[] { "CurrencyId1" });
            DropIndex("dbo.Trade", new[] { "TradePairId" });
            DropIndex("dbo.Trade", new[] { "UserId" });
            DropTable("dbo.TradeHistory");
            DropTable("dbo.TradePair");
            DropTable("dbo.Trade");
        }
    }
}
