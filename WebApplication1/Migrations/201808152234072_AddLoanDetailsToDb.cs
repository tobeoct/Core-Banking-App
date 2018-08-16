namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanDetailsToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoanDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Terms = c.String(nullable: false),
                        LoanAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LoanDetails");
        }
    }
}
