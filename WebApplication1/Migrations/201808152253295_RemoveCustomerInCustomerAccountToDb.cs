namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCustomerInCustomerAccountToDb : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerAccounts", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.CustomerAccounts", new[] { "Customer_Id" });
            DropColumn("dbo.CustomerAccounts", "CustomerId");
            DropColumn("dbo.CustomerAccounts", "Customer_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerAccounts", "Customer_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.CustomerAccounts", "CustomerId", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomerAccounts", "Customer_Id");
            AddForeignKey("dbo.CustomerAccounts", "Customer_Id", "dbo.Customers", "Id");
        }
    }
}
