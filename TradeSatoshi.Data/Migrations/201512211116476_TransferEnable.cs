namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransferEnable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Currency", "TransferFee", c => c.Decimal(nullable: false, precision: 38, scale: 8));
            AddColumn("dbo.AspNetUsers", "IsTransferEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsTransferEnabled");
            DropColumn("dbo.Currency", "TransferFee");
        }
    }
}
