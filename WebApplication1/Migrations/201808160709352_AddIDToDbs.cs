namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIDToDbs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanAccountTypes", "InterestIncomeGLAccountId", c => c.Int(nullable: false));
            AddColumn("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SavingsAccountTypes", "InterestExpenseGlAccountId");
            DropColumn("dbo.LoanAccountTypes", "InterestIncomeGLAccountId");
        }
    }
}
