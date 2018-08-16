namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountTypeToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        CreditInterestRate = c.Single(),
                        DebitInterestRate = c.Single(),
                        MinimumBalance = c.Single(),
                        InterestExpenseGLAccountId = c.Int(),
                        InterestIncomeGLAccountId = c.Int(),
                        COTIncomeGLAccountId = c.Int(),
                        COT = c.Single(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.COTIncomeGLAccountId)
                .ForeignKey("dbo.GLAccounts", t => t.InterestExpenseGLAccountId)
                .ForeignKey("dbo.GLAccounts", t => t.InterestIncomeGLAccountId)
                .Index(t => t.InterestExpenseGLAccountId)
                .Index(t => t.InterestIncomeGLAccountId)
                .Index(t => t.COTIncomeGLAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountTypes", "InterestIncomeGLAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.AccountTypes", "InterestExpenseGLAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.AccountTypes", "COTIncomeGLAccountId", "dbo.GLAccounts");
            DropIndex("dbo.AccountTypes", new[] { "COTIncomeGLAccountId" });
            DropIndex("dbo.AccountTypes", new[] { "InterestIncomeGLAccountId" });
            DropIndex("dbo.AccountTypes", new[] { "InterestExpenseGLAccountId" });
            DropTable("dbo.AccountTypes");
        }
    }
}
