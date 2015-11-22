namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailTemplates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Subject = c.String(maxLength: 256),
                        Template = c.String(maxLength: 4000),
                        IsHtml = c.Boolean(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.EmailTemplate");
        }
    }
}
