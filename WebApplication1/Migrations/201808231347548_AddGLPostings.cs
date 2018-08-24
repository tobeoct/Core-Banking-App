namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGLPostings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GLPostings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GlDebitAccountId = c.Int(nullable: false),
                        DebitAmount = c.Long(nullable: false),
                        DebitNarration = c.String(),
                        GlCreditAccountId = c.Int(nullable: false),
                        CreditAmount = c.Long(nullable: false),
                        CreditNarration = c.String(),
                        TransactionDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.GlCreditAccountId, cascadeDelete: false)
                .ForeignKey("dbo.GLAccounts", t => t.GlDebitAccountId, cascadeDelete: false)
                .Index(t => t.GlDebitAccountId)
                .Index(t => t.GlCreditAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GLPostings", "GlDebitAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.GLPostings", "GlCreditAccountId", "dbo.GLAccounts");
            DropIndex("dbo.GLPostings", new[] { "GlCreditAccountId" });
            DropIndex("dbo.GLPostings", new[] { "GlDebitAccountId" });
            DropTable("dbo.GLPostings");
        }
    }
}
