namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNarrationToFinancialReportsInDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinancialReports", "Narration", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinancialReports", "Narration");
        }
    }
}
