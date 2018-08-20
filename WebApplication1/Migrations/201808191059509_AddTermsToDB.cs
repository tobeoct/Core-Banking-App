namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTermsToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Terms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tenure = c.Single(nullable: false),
                        InterestRate = c.Single(nullable: false),
                        LoanAmount = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.LoanDetails", "TermsId", c => c.Int(nullable: false));
            CreateIndex("dbo.LoanDetails", "TermsId");
            AddForeignKey("dbo.LoanDetails", "TermsId", "dbo.Terms", "Id", cascadeDelete: true);
            DropColumn("dbo.LoanDetails", "Terms");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LoanDetails", "Terms", c => c.String(nullable: false));
            DropForeignKey("dbo.LoanDetails", "TermsId", "dbo.Terms");
            DropIndex("dbo.LoanDetails", new[] { "TermsId" });
            DropColumn("dbo.LoanDetails", "TermsId");
            DropTable("dbo.Terms");
        }
    }
}
