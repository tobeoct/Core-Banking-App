namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanAccountTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.LoanAccountTypes",
                    c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DebitInterestRate = c.Single(nullable: false),
                        
                        GlAccountId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.GlAccountId, cascadeDelete:true)
                .Index(t => t.GlAccountId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoanAccountTypes", "GlAccountId", "dbo.GLAccounts");
            DropIndex("dbo.LoanAccountTypes", new[] { "GlAccountId" });
            DropTable("dbo.LoanAccountTypes");
        }
    }
}
