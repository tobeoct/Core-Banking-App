namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeFieldsToLoanDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanDetails", "LinkedCustomerAccountBank", c => c.String());
            AddColumn("dbo.LoanDetails", "LinkedCustomerAccountNumber", c => c.Long(nullable: false));
            AddColumn("dbo.LoanDetails", "BVN", c => c.Long(nullable: false));
            DropColumn("dbo.LoanDetails", "linkedCustomerAccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LoanDetails", "linkedCustomerAccountId", c => c.Int(nullable: false));
            DropColumn("dbo.LoanDetails", "BVN");
            DropColumn("dbo.LoanDetails", "LinkedCustomerAccountNumber");
            DropColumn("dbo.LoanDetails", "LinkedCustomerAccountBank");
        }
    }
}
