namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTellerPostingsToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TellerPostings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostingType = c.String(nullable: false),
                        CustomerAccountId = c.Int(nullable: false),
                        Amount = c.Long(nullable: false),
                        Narration = c.String(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerAccounts", t => t.CustomerAccountId, cascadeDelete: true)
                .Index(t => t.CustomerAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TellerPostings", "CustomerAccountId", "dbo.CustomerAccounts");
            DropIndex("dbo.TellerPostings", new[] { "CustomerAccountId" });
            DropTable("dbo.TellerPostings");
        }
    }
}
