namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Address : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CurrencyId = c.Int(nullable: false),
                        AddressHash = c.String(maxLength: 256),
                        PrivateKey = c.String(maxLength: 512),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currency", t => t.CurrencyId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.CurrencyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Address", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Address", "CurrencyId", "dbo.Currency");
            DropIndex("dbo.Address", new[] { "CurrencyId" });
            DropIndex("dbo.Address", new[] { "UserId" });
            DropTable("dbo.Address");
        }
    }
}
