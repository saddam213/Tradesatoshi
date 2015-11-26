namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TFAUniqueConstraint : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserTwoFactor", new[] { "UserId" });
            CreateIndex("dbo.UserTwoFactor", new[] { "UserId", "Component" }, unique: true, name: "IX_UserComponent");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserTwoFactor", "IX_UserComponent");
            CreateIndex("dbo.UserTwoFactor", "UserId");
        }
    }
}
