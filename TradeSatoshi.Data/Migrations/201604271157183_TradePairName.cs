namespace TradeSatoshi.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class TradePairName : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.TradePair", "Name", c => c.String(maxLength: 50));
		}

		public override void Down()
		{
			DropColumn("dbo.TradePair", "Name");
		}
	}
}