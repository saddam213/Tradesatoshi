namespace TradeSatoshi.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class MarketSortOrder : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Currency", "MarketSortOrder", c => c.Int(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.Currency", "MarketSortOrder");
		}
	}
}
