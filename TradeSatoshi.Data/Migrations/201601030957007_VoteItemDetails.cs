namespace TradeSatoshi.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoteItemDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoteItem", "Symbol", c => c.String(maxLength: 10));
            AddColumn("dbo.VoteItem", "Website", c => c.String(maxLength: 128));
            AddColumn("dbo.VoteItem", "Source", c => c.String(maxLength: 128));
            AddColumn("dbo.VoteItem", "Description", c => c.String(maxLength: 500));
            AddColumn("dbo.VoteItem", "AlgoType", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoteItem", "AlgoType");
            DropColumn("dbo.VoteItem", "Description");
            DropColumn("dbo.VoteItem", "Source");
            DropColumn("dbo.VoteItem", "Website");
            DropColumn("dbo.VoteItem", "Symbol");
        }
    }
}
