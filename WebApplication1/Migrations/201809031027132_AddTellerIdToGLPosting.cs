namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTellerIdToGLPosting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GLPostings", "TellerId", c => c.Int(nullable: false));
            CreateIndex("dbo.GLPostings", "TellerId");
            AddForeignKey("dbo.GLPostings", "TellerId", "dbo.Tellers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GLPostings", "TellerId", "dbo.Tellers");
            DropIndex("dbo.GLPostings", new[] { "TellerId" });
            DropColumn("dbo.GLPostings", "TellerId");
        }
    }
}
