namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountBalanceToCustomerAccountDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerAccounts", "AccountBalance", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerAccounts", "AccountBalance");
        }
    }
}
