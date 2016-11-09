namespace TradeSatoshi.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class CurrencyChanges : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.Currency", new[] { "Symbol" });
			AddColumn("dbo.Currency", "Algo", c => c.String(maxLength: 128));
			AddColumn("dbo.Currency", "Type", c => c.Int(nullable: false));
			AddColumn("dbo.Currency", "InterfaceType", c => c.Int(nullable: false));
			CreateIndex("dbo.Currency", "Symbol", unique: true);
		}

		public override void Down()
		{
			DropIndex("dbo.Currency", new[] { "Symbol" });
			DropColumn("dbo.Currency", "InterfaceType");
			DropColumn("dbo.Currency", "Type");
			DropColumn("dbo.Currency", "Algo");
			CreateIndex("dbo.Currency", "Symbol");
		}
	}
}
