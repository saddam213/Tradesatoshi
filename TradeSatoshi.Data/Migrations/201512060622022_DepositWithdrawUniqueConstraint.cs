namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepositWithdrawUniqueConstraint : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Deposit", new[] { "UserId" });
            CreateIndex("dbo.Deposit", new[] { "UserId", "Txid" }, unique: true, name: "IX_UserTxId");
            CreateIndex("dbo.Withdraw", "Txid", unique: true, name: "IX_TxId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Withdraw", "IX_TxId");
            DropIndex("dbo.Deposit", "IX_UserTxId");
            CreateIndex("dbo.Deposit", "UserId");
        }
    }
}
