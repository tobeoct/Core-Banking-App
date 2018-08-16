namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerInCustomerAccountToDb3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerAccounts", "CustomerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CustomerAccounts", "CustomerId");
            AddForeignKey("dbo.CustomerAccounts", "CustomerId", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerAccounts", "CustomerId", "dbo.Customers");
            DropIndex("dbo.CustomerAccounts", new[] { "CustomerId" });
            DropColumn("dbo.CustomerAccounts", "CustomerId");
        }
    }
}
