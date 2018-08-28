namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryToFinancialReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinancialReports", "DebitAccountCategory", c => c.String());
            AddColumn("dbo.FinancialReports", "CreditAccountCategory", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinancialReports", "CreditAccountCategory");
            DropColumn("dbo.FinancialReports", "DebitAccountCategory");
        }
    }
}
