namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSavingsAccountTypeToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SavingsAccountTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreditInterestRate = c.Single(nullable: false),
                        MinimumBalance = c.Single(nullable: false),
                        GlAccount_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.GlAccount_Id)
                .Index(t => t.GlAccount_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavingsAccountTypes", "GlAccount_Id", "dbo.GLAccounts");
            DropIndex("dbo.SavingsAccountTypes", new[] { "GlAccount_Id" });
            DropTable("dbo.SavingsAccountTypes");
        }
    }
}
