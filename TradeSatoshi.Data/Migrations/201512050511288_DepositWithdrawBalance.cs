namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepositWithdrawBalance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Balance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        CurrencyId = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Unconfirmed = c.Decimal(nullable: false, precision: 38, scale: 8),
                        HeldForTrades = c.Decimal(nullable: false, precision: 38, scale: 8),
                        PendingWithdraw = c.Decimal(nullable: false, precision: 38, scale: 8),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currency", t => t.CurrencyId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CurrencyId);
            
            CreateTable(
                "dbo.Deposit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        CurrencyId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Txid = c.String(maxLength: 256),
                        Confirmations = c.Int(nullable: false),
                        DepositType = c.Int(nullable: false),
                        DepositStatus = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currency", t => t.CurrencyId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CurrencyId);
            
            CreateTable(
                "dbo.Withdraw",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        CurrencyId = c.Int(nullable: false),
                        Address = c.String(maxLength: 256),
                        Amount = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Fee = c.Decimal(nullable: false, precision: 38, scale: 8),
                        Confirmations = c.Int(nullable: false),
                        Txid = c.String(maxLength: 256),
                        WithdrawType = c.Int(nullable: false),
                        WithdrawStatus = c.Int(nullable: false),
                        TwoFactorToken = c.String(maxLength: 1024),
                        IsApi = c.Boolean(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currency", t => t.CurrencyId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CurrencyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Withdraw", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Withdraw", "CurrencyId", "dbo.Currency");
            DropForeignKey("dbo.Deposit", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Deposit", "CurrencyId", "dbo.Currency");
            DropForeignKey("dbo.Balance", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Balance", "CurrencyId", "dbo.Currency");
            DropIndex("dbo.Withdraw", new[] { "CurrencyId" });
            DropIndex("dbo.Withdraw", new[] { "UserId" });
            DropIndex("dbo.Deposit", new[] { "CurrencyId" });
            DropIndex("dbo.Deposit", new[] { "UserId" });
            DropIndex("dbo.Balance", new[] { "CurrencyId" });
            DropIndex("dbo.Balance", new[] { "UserId" });
            DropTable("dbo.Withdraw");
            DropTable("dbo.Deposit");
            DropTable("dbo.Balance");
        }
    }
}
