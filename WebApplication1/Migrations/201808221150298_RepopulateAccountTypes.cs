namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RepopulateAccountTypes : DbMigration
    {
        public override void Up()
        {
//            Sql(@"
//                INSERT INTO AccountTypes (Name,CreditInterestRate,DebitInterestRate,MinimumBalance,InterestExpenseGLAccountId,InterestIncomeGLAccountId,COTIncomeGLAccountId,COT) VALUES ('Savings Account',0.5,NULL,1000,6,NULL,NULL,NULL)
//                INSERT INTO AccountTypes (Name,CreditInterestRate,DebitInterestRate,MinimumBalance,InterestExpenseGLAccountId,InterestIncomeGLAccountId,COTIncomeGLAccountId,COT) VALUES ('Current Account',0.5,NULL,1000,6,NULL,8,10)
//                INSERT INTO AccountTypes (Name,CreditInterestRate,DebitInterestRate,MinimumBalance,InterestExpenseGLAccountId,InterestIncomeGLAccountId,COTIncomeGLAccountId,COT) VALUES ('Loan Account',NULL,0.5,NULL,NULL,7,NULL,NULL)
//                ");
        }
        
        public override void Down()
        {
        }
    }
}
