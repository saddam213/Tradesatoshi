namespace TradeSatoshi.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class ColdBalance : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Currency", "ColdBalance", c => c.Decimal(nullable: false, precision: 38, scale: 8));
		}

		public override void Down()
		{
			DropColumn("dbo.Currency", "ColdBalance");
		}
	}
}
