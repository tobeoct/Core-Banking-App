namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLoanAcc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanAccountTypes", "InterestIncomeGLAccountId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoanAccountTypes", "InterestIncomeGLAccountId");
        }
    }
}
