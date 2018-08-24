namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeMig3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.CustomerAccounts",
                    c => new
                    {
                        Id = c.Int(nullable: false,identity:true),
                        Name = c.String(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        AccountNumber = c.String(nullable: false),
                        AccountBalance = c.Long(nullable: false),
                        BranchId = c.Int(nullable: false),
                        AccountTypeId = c.Int(nullable: false),
                        LoanDetailsId = c.Int(),
                        CustomerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.AccountTypes", t => t.AccountTypeId, cascadeDelete: true)
                .ForeignKey("dbo.LoanDetails", t => t.LoanDetailsId)
                .Index(t => t.BranchId)
                .Index(t => t.AccountTypeId)
                .Index(t => t.LoanDetailsId)
                .Index(t => t.CustomerId);
        }
        
        public override void Down()
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
    }
}
