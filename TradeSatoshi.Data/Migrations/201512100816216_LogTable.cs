namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Log",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(maxLength: 128),
                        Component = c.String(maxLength: 128),
                        Message = c.String(maxLength: 4000),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Log");
        }
    }
}
