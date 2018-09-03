namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInterestToCustomerAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerAccounts", "Interest", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerAccounts", "Interest");
        }
    }
}
