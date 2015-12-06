namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TwoFactorUniqueConstraint : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserTwoFactor", "IX_UserComponent");
            AlterColumn("dbo.UserTwoFactor", "Component", c => c.Int(nullable: false));
            AlterColumn("dbo.UserTwoFactor", "Type", c => c.Int(nullable: false));
            CreateIndex("dbo.UserTwoFactor", new[] { "UserId", "Component" }, unique: true, name: "IX_UserComponent");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserTwoFactor", "IX_UserComponent");
            AlterColumn("dbo.UserTwoFactor", "Type", c => c.Byte(nullable: false));
            AlterColumn("dbo.UserTwoFactor", "Component", c => c.Byte(nullable: false));
            CreateIndex("dbo.UserTwoFactor", new[] { "UserId", "Component" }, unique: true, name: "IX_UserComponent");
        }
    }
}
