namespace TradeSatoshi.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class VoteSettings1 : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.VoteSettings", "CurrencyId", c => c.Int(nullable: false));
			AddColumn("dbo.VoteSettings", "Price", c => c.Decimal(nullable: false, precision: 38, scale: 8));
			AddColumn("dbo.VoteSettings", "IsFreeEnabled", c => c.Boolean(nullable: false));
			AddColumn("dbo.VoteSettings", "IsPaidEnabled", c => c.Boolean(nullable: false));
			CreateIndex("dbo.VoteSettings", "CurrencyId");
			AddForeignKey("dbo.VoteSettings", "CurrencyId", "dbo.Currency", "Id", cascadeDelete: false);
		}

		public override void Down()
		{
			DropForeignKey("dbo.VoteSettings", "CurrencyId", "dbo.Currency");
			DropIndex("dbo.VoteSettings", new[] { "CurrencyId" });
			DropColumn("dbo.VoteSettings", "IsPaidEnabled");
			DropColumn("dbo.VoteSettings", "IsFreeEnabled");
			DropColumn("dbo.VoteSettings", "Price");
			DropColumn("dbo.VoteSettings", "CurrencyId");
		}
	}
}
