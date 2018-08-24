namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeMig : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CustomerAccounts");
            AlterColumn("dbo.CustomerAccounts", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.CustomerAccounts", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CustomerAccounts");
            AlterColumn("dbo.CustomerAccounts", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.CustomerAccounts", "Id");
        }
    }
}
