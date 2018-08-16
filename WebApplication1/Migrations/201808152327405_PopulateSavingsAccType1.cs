namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateSavingsAccType1 : DbMigration
    {
        public override void Up()
        {
//            Sql("INSERT INTO SavingsAccountTypes (Id,CreditInterestRate,MinimumBalance,InterestExpenseGLAccountId) VALUES ('Savings Account',0.5,1000,1)");
//            Sql("INSERT INTO LoanAccountTypes (Id,DebitInterestRate,GLAccountId) VALUES ('Savings Account',5.0,1)");
        }
        
        public override void Down()
        {
        }
    }
}
