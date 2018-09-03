namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserIdToGLPosting : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GLPostings", "TellerId", "dbo.Tellers");
            DropIndex("dbo.GLPostings", new[] { "TellerId" });
            AddColumn("dbo.GLPostings", "UserAccountId", c => c.Int(nullable: false));
            AddColumn("dbo.GLPostings", "UserAccount_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.GLPostings", "UserAccount_Id");
            AddForeignKey("dbo.GLPostings", "UserAccount_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.GLPostings", "TellerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GLPostings", "TellerId", c => c.Int(nullable: false));
            DropForeignKey("dbo.GLPostings", "UserAccount_Id", "dbo.AspNetUsers");
            DropIndex("dbo.GLPostings", new[] { "UserAccount_Id" });
            DropColumn("dbo.GLPostings", "UserAccount_Id");
            DropColumn("dbo.GLPostings", "UserAccountId");
            CreateIndex("dbo.GLPostings", "TellerId");
            AddForeignKey("dbo.GLPostings", "TellerId", "dbo.Tellers", "Id", cascadeDelete: true);
        }
    }
}
