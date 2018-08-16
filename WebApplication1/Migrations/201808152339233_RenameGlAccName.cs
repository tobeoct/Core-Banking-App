namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameGlAccName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId", "dbo.GLAccounts");
            DropIndex("dbo.LoanAccountTypes", new[] { "GlAccountId" });
            DropIndex("dbo.SavingsAccountTypes", new[] { "InterestExpenseGlAccountId" });
            AddColumn("dbo.SavingsAccountTypes", "GLAccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.LoanAccountTypes", "GLAccountId");
            CreateIndex("dbo.SavingsAccountTypes", "GLAccountId");
            AddForeignKey("dbo.SavingsAccountTypes", "GLAccountId", "dbo.GLAccounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavingsAccountTypes", "GLAccountId", "dbo.GLAccounts");
            DropIndex("dbo.SavingsAccountTypes", new[] { "GLAccountId" });
            DropIndex("dbo.LoanAccountTypes", new[] { "GLAccountId" });
            DropColumn("dbo.SavingsAccountTypes", "GLAccountId");
            CreateIndex("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId");
            CreateIndex("dbo.LoanAccountTypes", "GlAccountId");
            AddForeignKey("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId", "dbo.GLAccounts", "Id", cascadeDelete: true);
        }
    }
}
