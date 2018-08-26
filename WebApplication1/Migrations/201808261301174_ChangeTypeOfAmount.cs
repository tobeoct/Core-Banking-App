namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeOfAmount : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FinancialReports", "DebitAmount", c => c.Single(nullable: false));
            AlterColumn("dbo.FinancialReports", "CreditAmount", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FinancialReports", "CreditAmount", c => c.Long(nullable: false));
            AlterColumn("dbo.FinancialReports", "DebitAmount", c => c.Long(nullable: false));
        }
    }
}
