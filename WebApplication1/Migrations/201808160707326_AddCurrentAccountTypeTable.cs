namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCurrentAccountTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.CurrentAccountTypes",
                    c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreditInterestRate = c.Single(nullable: false),
                        MinimumBalance = c.Single(nullable: false),
                        GLAccountId = c.Int(nullable: false),
                        InterestExpenseGLAccountId = c.Int(nullable: false),
                        COT = c.Int(nullable: false),
                        COTIncomeGLAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.GLAccountId, cascadeDelete: true)
                .Index(t => t.GLAccountId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CurrentAccountTypes", "GLAccountId", "dbo.GLAccounts");
            DropIndex("dbo.CurrentAccountTypes", new[] { "GLAccountId" });
            DropTable("dbo.CurrentAccountTypes");
        }
    }
}
