namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateAccType : DbMigration
    {
        public override void Up()
        {
             Sql("INSERT INTO SavingsAccountTypes (Id,CreditInterestRate,MinimumBalance,GlAccountId,InterestExpenseGLAccountId) VALUES ('Savings Account',0.5,1000,2,2)");
            Sql("INSERT INTO LoanAccountTypes (Id,DebitInterestRate,GLAccountId,InterestIncomeGLAccountId) VALUES ('Loan Account',5.0,2,2)");
        }

        public override void Down()
        {
        }
    }
}
