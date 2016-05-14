namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndexesRegisterDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RegisterDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Currency", "Symbol");
            CreateIndex("dbo.Currency", "Status");
            CreateIndex("dbo.Deposit", "DepositStatus", name: "IX_Status");
            CreateIndex("dbo.Trade", "TradeType", name: "IX_Type");
            CreateIndex("dbo.Trade", "Status");
            CreateIndex("dbo.TradePair", "Name");
            CreateIndex("dbo.Withdraw", "WithdrawStatus", name: "IX_Status");
            CreateIndex("dbo.TradeHistory", "TradeHistoryType", name: "IX_Type");
            CreateIndex("dbo.Vote", "Type");
            CreateIndex("dbo.Vote", "Status");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Vote", new[] { "Status" });
            DropIndex("dbo.Vote", new[] { "Type" });
            DropIndex("dbo.TradeHistory", "IX_Type");
            DropIndex("dbo.Withdraw", "IX_Status");
            DropIndex("dbo.TradePair", new[] { "Name" });
            DropIndex("dbo.Trade", new[] { "Status" });
            DropIndex("dbo.Trade", "IX_Type");
            DropIndex("dbo.Deposit", "IX_Status");
            DropIndex("dbo.Currency", new[] { "Status" });
            DropIndex("dbo.Currency", new[] { "Symbol" });
            DropColumn("dbo.AspNetUsers", "RegisterDate");
        }
    }
}
