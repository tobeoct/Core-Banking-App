namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerAccountToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerAccounts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        AccountNumber = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        CustomerAccountTypeId = c.Int(nullable: false),
                        LoanDetailsId = c.Int(),
                        Customer_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .ForeignKey("dbo.CustomerAccountTypes", t => t.CustomerAccountTypeId, cascadeDelete: true)
                .ForeignKey("dbo.LoanDetails", t => t.LoanDetailsId)
                .Index(t => t.BranchId)
                .Index(t => t.CustomerAccountTypeId)
                .Index(t => t.LoanDetailsId)
                .Index(t => t.Customer_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerAccounts", "LoanDetailsId", "dbo.LoanDetails");
            DropForeignKey("dbo.CustomerAccounts", "CustomerAccountTypeId", "dbo.CustomerAccountTypes");
            DropForeignKey("dbo.CustomerAccounts", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.CustomerAccounts", "BranchId", "dbo.Branches");
            DropIndex("dbo.CustomerAccounts", new[] { "Customer_Id" });
            DropIndex("dbo.CustomerAccounts", new[] { "LoanDetailsId" });
            DropIndex("dbo.CustomerAccounts", new[] { "CustomerAccountTypeId" });
            DropIndex("dbo.CustomerAccounts", new[] { "BranchId" });
            DropTable("dbo.CustomerAccounts");
        }
    }
}
