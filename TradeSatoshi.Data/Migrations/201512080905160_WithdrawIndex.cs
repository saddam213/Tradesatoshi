namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WithdrawIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Withdraw", new[] { "UserId" });
            DropIndex("dbo.Withdraw", new[] { "CurrencyId" });
            DropIndex("dbo.Withdraw", "IX_TxId");
            CreateIndex("dbo.Withdraw", "UserId");
            CreateIndex("dbo.Withdraw", "CurrencyId");
            CreateIndex("dbo.Withdraw", "Txid", name: "IX_TxId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Withdraw", "IX_TxId");
            DropIndex("dbo.Withdraw", new[] { "CurrencyId" });
            DropIndex("dbo.Withdraw", new[] { "UserId" });
            CreateIndex("dbo.Withdraw", "Txid", unique: true, name: "IX_TxId");
            CreateIndex("dbo.Withdraw", "CurrencyId");
            CreateIndex("dbo.Withdraw", "UserId");
        }
    }
}
