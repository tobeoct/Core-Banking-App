namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCOTToCustomerAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerAccounts", "COTIncome", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerAccounts", "COTIncome");
        }
    }
}
