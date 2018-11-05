namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionLogs", "RemoteOnUs", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionLogs", "RemoteOnUs");
        }
    }
}
