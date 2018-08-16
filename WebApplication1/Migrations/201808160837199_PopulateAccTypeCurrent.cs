namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateAccTypeCurrent : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO CurrentAccountTypes (Id,CreditInterestRate,MinimumBalance,GlAccountId,InterestExpenseGLAccountId,COT,COTIncomeGLAccountId) VALUES ('Current Account',0.5,1000,2,2,10,3)");
           
        }
    
        
        public override void Down()
        {
        }
    }
}
