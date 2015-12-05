namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Currency : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                        Symbol = c.String(maxLength: 128),
                        Balance = c.Decimal(nullable: false, precision: 38, scale: 8),
                        WalletUser = c.String(maxLength: 128),
                        WalletPass = c.String(maxLength: 128),
                        WalletPort = c.Int(nullable: false),
                        WalletHost = c.String(maxLength: 128),
                        TradeFee = c.Decimal(nullable: false, precision: 38, scale: 8),
                        MinTrade = c.Decimal(nullable: false, precision: 38, scale: 8),
                        MaxTrade = c.Decimal(nullable: false, precision: 38, scale: 8),
                        MinBaseTrade = c.Decimal(nullable: false, precision: 38, scale: 8),
                        WithdrawFee = c.Decimal(nullable: false, precision: 38, scale: 8),
                        WithdrawFeeType = c.Int(nullable: false),
                        MinWithdraw = c.Decimal(nullable: false, precision: 38, scale: 8),
                        MaxWithdraw = c.Decimal(nullable: false, precision: 38, scale: 8),
                        MinConfirmations = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        StatusMessage = c.String(maxLength: 1024),
                        LastBlockHash = c.String(maxLength: 256),
                        LastWithdrawBlockHash = c.String(maxLength: 256),
                        Block = c.Int(nullable: false),
                        Version = c.String(maxLength: 128),
                        Connections = c.Int(nullable: false),
                        Errors = c.String(maxLength: 1024),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Currency");
        }
    }
}
