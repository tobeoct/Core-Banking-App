namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinancialDateTODb3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FinancialDates", "EOM", c => c.DateTime());
            AlterColumn("dbo.FinancialDates", "EOY", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FinancialDates", "EOY", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FinancialDates", "EOM", c => c.DateTime(nullable: false));
        }
    }
}
