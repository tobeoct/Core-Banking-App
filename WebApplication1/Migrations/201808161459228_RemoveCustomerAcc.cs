namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCustomerAcc : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerAccounts", "AccountTypeId", "dbo.AccountTypes");
            DropForeignKey("dbo.CustomerAccounts", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.CustomerAccounts", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.CustomerAccounts", "LoanDetailsId", "dbo.LoanDetails");
            DropIndex("dbo.CustomerAccounts", new[] { "CustomerId" });
            DropIndex("dbo.CustomerAccounts", new[] { "BranchId" });
            DropIndex("dbo.CustomerAccounts", new[] { "AccountTypeId" });
            DropIndex("dbo.CustomerAccounts", new[] { "LoanDetailsId" });
            DropTable("dbo.CustomerAccounts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CustomerAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        AccountNumber = c.Int(nullable: false),
                        CustomerId = c.String(maxLength: 128),
                        BranchId = c.Int(nullable: false),
                        AccountTypeId = c.Int(nullable: false),
                        LoanDetailsId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.CustomerAccounts", "LoanDetailsId");
            CreateIndex("dbo.CustomerAccounts", "AccountTypeId");
            CreateIndex("dbo.CustomerAccounts", "BranchId");
            CreateIndex("dbo.CustomerAccounts", "CustomerId");
            AddForeignKey("dbo.CustomerAccounts", "LoanDetailsId", "dbo.LoanDetails", "Id");
            AddForeignKey("dbo.CustomerAccounts", "CustomerId", "dbo.Customers", "Id");
            AddForeignKey("dbo.CustomerAccounts", "BranchId", "dbo.Branches", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomerAccounts", "AccountTypeId", "dbo.AccountTypes", "Id", cascadeDelete: true);
        }
    }
}
