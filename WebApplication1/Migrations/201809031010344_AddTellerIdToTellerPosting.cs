namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTellerIdToTellerPosting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TellerPostings", "TellerId", c => c.Int(nullable: false));
            CreateIndex("dbo.TellerPostings", "TellerId");
            AddForeignKey("dbo.TellerPostings", "TellerId", "dbo.Tellers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TellerPostings", "TellerId", "dbo.Tellers");
            DropIndex("dbo.TellerPostings", new[] { "TellerId" });
            DropColumn("dbo.TellerPostings", "TellerId");
        }
    }
}
