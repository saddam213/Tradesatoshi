namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Balance : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Balance", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Deposit", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Withdraw", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Balance", new[] { "UserId" });
            DropIndex("dbo.Deposit", new[] { "UserId" });
            DropIndex("dbo.Withdraw", new[] { "UserId" });
            AlterColumn("dbo.Balance", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Deposit", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Withdraw", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Balance", "UserId");
            CreateIndex("dbo.Deposit", "UserId");
			CreateIndex("dbo.Withdraw", "UserId");
            AddForeignKey("dbo.Balance", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Deposit", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Withdraw", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Withdraw", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Deposit", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Balance", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Withdraw", new[] { "UserId" });
            DropIndex("dbo.Deposit", new[] { "UserId" });
            DropIndex("dbo.Balance", new[] { "UserId" });
            AlterColumn("dbo.Withdraw", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Deposit", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Balance", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Withdraw", "UserId");
            CreateIndex("dbo.Deposit", "UserId");
            CreateIndex("dbo.Balance", "UserId");
            AddForeignKey("dbo.Withdraw", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Deposit", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Balance", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
