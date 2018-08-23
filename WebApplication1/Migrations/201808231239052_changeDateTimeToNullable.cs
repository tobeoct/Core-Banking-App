namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeDateTimeToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FinancialReports", "ReportDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FinancialReports", "ReportDate", c => c.DateTime(nullable: false));
        }
    }
}
