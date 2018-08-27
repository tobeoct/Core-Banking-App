namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinancialDateTODb2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinancialDates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EOD = c.DateTime(nullable: false),
                        EOM = c.DateTime(nullable: false),
                        EOY = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FinancialDates");
        }
    }
}
