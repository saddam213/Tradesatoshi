namespace TradeSatoshi.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class EmailParameters : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.EmailTemplate", "From", c => c.String(maxLength: 256));
			AddColumn("dbo.EmailTemplate", "Description", c => c.String(maxLength: 1000));
			AddColumn("dbo.EmailTemplate", "Parameters", c => c.String(maxLength: 1000));
		}

		public override void Down()
		{
			DropColumn("dbo.EmailTemplate", "Parameters");
			DropColumn("dbo.EmailTemplate", "Description");
			DropColumn("dbo.EmailTemplate", "From");
		}
	}
}
