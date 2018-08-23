namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDb : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GLAccounts", "CategoriesId", "dbo.Categories");
            DropForeignKey("dbo.CurrentAccountTypes", "GLAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.CustomerAccountTypes", "CurrentAccountTypeId", "dbo.CurrentAccountTypes");
            DropForeignKey("dbo.LoanAccountTypes", "GlAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.CustomerAccountTypes", "LoanAccountTypeId", "dbo.LoanAccountTypes");
            DropForeignKey("dbo.SavingsAccountTypes", "GLAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.CustomerAccountTypes", "SavingsAccountTypeId", "dbo.SavingsAccountTypes");
            DropIndex("dbo.GLAccounts", new[] { "CategoriesId" });
            DropIndex("dbo.CurrentAccountTypes", new[] { "GLAccountId" });
            DropIndex("dbo.CustomerAccountTypes", new[] { "SavingsAccountTypeId" });
            DropIndex("dbo.CustomerAccountTypes", new[] { "CurrentAccountTypeId" });
            DropIndex("dbo.CustomerAccountTypes", new[] { "LoanAccountTypeId" });
            DropIndex("dbo.LoanAccountTypes", new[] { "GlAccountId" });
            DropIndex("dbo.SavingsAccountTypes", new[] { "GLAccountId" });
            AddColumn("dbo.GLAccounts", "GlCategoriesId", c => c.Byte(nullable: false));
            AddColumn("dbo.GLAccounts", "GlCategories_Id", c => c.Int());
            CreateIndex("dbo.GLAccounts", "GlCategories_Id");
            AddForeignKey("dbo.GLAccounts", "GlCategories_Id", "dbo.GLCategories", "Id");
            DropColumn("dbo.GLAccounts", "CategoriesId");
            DropTable("dbo.CurrentAccountTypes");
            DropTable("dbo.CustomerAccountTypes");
            DropTable("dbo.LoanAccountTypes");
            DropTable("dbo.SavingsAccountTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SavingsAccountTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreditInterestRate = c.Single(nullable: false),
                        MinimumBalance = c.Single(nullable: false),
                        InterestExpenseGlAccountId = c.Int(nullable: false),
                        GLAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoanAccountTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DebitInterestRate = c.Single(nullable: false),
                        InterestIncomeGLAccountId = c.Int(nullable: false),
                        GlAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerAccountTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SavingsAccountTypeId = c.String(maxLength: 128),
                        CurrentAccountTypeId = c.String(maxLength: 128),
                        LoanAccountTypeId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.GLAccounts", "CategoriesId", c => c.Byte(nullable: false));
            DropForeignKey("dbo.GLAccounts", "GlCategories_Id", "dbo.GLCategories");
            DropIndex("dbo.GLAccounts", new[] { "GlCategories_Id" });
            DropColumn("dbo.GLAccounts", "GlCategories_Id");
            DropColumn("dbo.GLAccounts", "GlCategoriesId");
            CreateIndex("dbo.SavingsAccountTypes", "GLAccountId");
            CreateIndex("dbo.LoanAccountTypes", "GlAccountId");
            CreateIndex("dbo.CustomerAccountTypes", "LoanAccountTypeId");
            CreateIndex("dbo.CustomerAccountTypes", "CurrentAccountTypeId");
            CreateIndex("dbo.CustomerAccountTypes", "SavingsAccountTypeId");
            CreateIndex("dbo.CurrentAccountTypes", "GLAccountId");
            CreateIndex("dbo.GLAccounts", "CategoriesId");
            AddForeignKey("dbo.CustomerAccountTypes", "SavingsAccountTypeId", "dbo.SavingsAccountTypes", "Id");
            AddForeignKey("dbo.SavingsAccountTypes", "GLAccountId", "dbo.GLAccounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomerAccountTypes", "LoanAccountTypeId", "dbo.LoanAccountTypes", "Id");
            AddForeignKey("dbo.LoanAccountTypes", "GlAccountId", "dbo.GLAccounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomerAccountTypes", "CurrentAccountTypeId", "dbo.CurrentAccountTypes", "Id");
            AddForeignKey("dbo.CurrentAccountTypes", "GLAccountId", "dbo.GLAccounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GLAccounts", "CategoriesId", "dbo.Categories", "Id", cascadeDelete: true);
        }
    }
}
