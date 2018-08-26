namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInterestRateToLoanDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanDetails", "InterestRate", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoanDetails", "InterestRate");
        }
    }
}
