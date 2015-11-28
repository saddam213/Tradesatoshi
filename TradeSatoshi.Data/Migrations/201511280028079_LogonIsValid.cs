namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogonIsValid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogon", "IsValid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogon", "IsValid");
        }
    }
}
