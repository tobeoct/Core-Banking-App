namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollateralToTermsInDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Terms", "Collateral", c => c.String());
            DropColumn("dbo.Terms", "InterestRate");
            DropColumn("dbo.Terms", "LoanAmount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Terms", "LoanAmount", c => c.Single(nullable: false));
            AddColumn("dbo.Terms", "InterestRate", c => c.Single(nullable: false));
            DropColumn("dbo.Terms", "Collateral");
        }
    }
}
