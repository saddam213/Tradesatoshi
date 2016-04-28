namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApiKetSecret : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ApiKey", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "ApiSecret", c => c.String(maxLength: 256));
            AddColumn("dbo.AspNetUsers", "IsApiEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsApiEnabled");
            DropColumn("dbo.AspNetUsers", "ApiSecret");
            DropColumn("dbo.AspNetUsers", "ApiKey");
        }
    }
}
