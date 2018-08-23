namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinancialReportToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinancialReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DebitAccount = c.String(),
                        DebitAmount = c.String(),
                        CreditAccount = c.String(),
                        CreditAmount = c.String(),
                        PostingType = c.String(),
                        ReportDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FinancialReports");
        }
    }
}
