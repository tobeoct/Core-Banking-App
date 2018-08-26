namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsToLoanDetailsInDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanDetails", "InterestReceivable", c => c.Single(nullable: false));
            AddColumn("dbo.LoanDetails", "InterestIncome", c => c.Single(nullable: false));
            AddColumn("dbo.LoanDetails", "PrincipalOverdue", c => c.Single(nullable: false));
            AddColumn("dbo.LoanDetails", "InterestInSuspense", c => c.Single(nullable: false));
            AddColumn("dbo.LoanDetails", "InterestOverdue", c => c.Single(nullable: false));
            AddColumn("dbo.LoanDetails", "CustomerLoan", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoanDetails", "CustomerLoan");
            DropColumn("dbo.LoanDetails", "InterestOverdue");
            DropColumn("dbo.LoanDetails", "InterestInSuspense");
            DropColumn("dbo.LoanDetails", "PrincipalOverdue");
            DropColumn("dbo.LoanDetails", "InterestIncome");
            DropColumn("dbo.LoanDetails", "InterestReceivable");
        }
    }
}
