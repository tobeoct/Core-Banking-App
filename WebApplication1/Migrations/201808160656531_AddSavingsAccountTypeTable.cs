namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSavingsAccountTypeTable : DbMigration
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
                        GlAccountId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.GlAccountId, cascadeDelete: true)
                .Index(t => t.GlAccountId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavingsAccountTypes", "GlAccountId", "dbo.GLAccounts");
            DropIndex("dbo.SavingsAccountTypes", new[] { "GlAccountId" });
            DropTable("dbo.SavingsAccountTypes");
        }
    }
}
