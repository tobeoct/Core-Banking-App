namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSomeTables : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.LoanAccountTypes", new[] { "GLAccountId" });
            DropIndex("dbo.SavingsAccountTypes", new[] { "GLAccountId" });
            CreateIndex("dbo.LoanAccountTypes", "GlAccountId");
            CreateIndex("dbo.SavingsAccountTypes", "GlAccountId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SavingsAccountTypes", new[] { "GlAccountId" });
            DropIndex("dbo.LoanAccountTypes", new[] { "GlAccountId" });
            CreateIndex("dbo.SavingsAccountTypes", "GLAccountId");
            CreateIndex("dbo.LoanAccountTypes", "GLAccountId");
        }
    }
}
