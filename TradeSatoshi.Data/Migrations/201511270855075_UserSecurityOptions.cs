namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSecurityOptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserLogon",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IPAddress = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            AddColumn("dbo.AspNetUsers", "IsEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsTradeEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsWithdrawEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserLogon", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserLogon", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "IsWithdrawEnabled");
            DropColumn("dbo.AspNetUsers", "IsTradeEnabled");
            DropColumn("dbo.AspNetUsers", "IsEnabled");
            DropTable("dbo.UserLogon");
        }
    }
}
