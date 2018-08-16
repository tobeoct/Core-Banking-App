namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanAccountTypeToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoanAccountTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DebitInterestRate = c.Single(nullable: false),
                        GlAccount_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.GlAccount_Id)
                .Index(t => t.GlAccount_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoanAccountTypes", "GlAccount_Id", "dbo.GLAccounts");
            DropIndex("dbo.LoanAccountTypes", new[] { "GlAccount_Id" });
            DropTable("dbo.LoanAccountTypes");
        }
    }
}
