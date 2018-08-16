namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GLAccountIdNameChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LoanAccountTypes", "GlAccount_Id", "dbo.GLAccounts");
            DropForeignKey("dbo.SavingsAccountTypes", "GlAccount_Id", "dbo.GLAccounts");
            DropIndex("dbo.LoanAccountTypes", new[] { "GlAccount_Id" });
            DropIndex("dbo.SavingsAccountTypes", new[] { "GlAccount_Id" });
            RenameColumn(table: "dbo.LoanAccountTypes", name: "GlAccount_Id", newName: "GlAccountId");
            RenameColumn(table: "dbo.SavingsAccountTypes", name: "GlAccount_Id", newName: "InterestExpenseGlAccountId");
            AlterColumn("dbo.LoanAccountTypes", "GlAccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.LoanAccountTypes", "GlAccountId");
            CreateIndex("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId");
            AddForeignKey("dbo.LoanAccountTypes", "GlAccountId", "dbo.GLAccounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId", "dbo.GLAccounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.LoanAccountTypes", "GlAccountId", "dbo.GLAccounts");
            DropIndex("dbo.SavingsAccountTypes", new[] { "InterestExpenseGlAccountId" });
            DropIndex("dbo.LoanAccountTypes", new[] { "GlAccountId" });
            AlterColumn("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId", c => c.Int());
            AlterColumn("dbo.LoanAccountTypes", "GlAccountId", c => c.Int());
            RenameColumn(table: "dbo.SavingsAccountTypes", name: "InterestExpenseGlAccountId", newName: "GlAccount_Id");
            RenameColumn(table: "dbo.LoanAccountTypes", name: "GlAccountId", newName: "GlAccount_Id");
            CreateIndex("dbo.SavingsAccountTypes", "GlAccount_Id");
            CreateIndex("dbo.LoanAccountTypes", "GlAccount_Id");
            AddForeignKey("dbo.SavingsAccountTypes", "GlAccount_Id", "dbo.GLAccounts", "Id");
            AddForeignKey("dbo.LoanAccountTypes", "GlAccount_Id", "dbo.GLAccounts", "Id");
        }
    }
}
