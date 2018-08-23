namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTellerToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tellers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        
                        TillAccountId = c.Int(nullable: false),
                        IsAssigned = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.TillAccountId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.TillAccountId)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tellers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tellers", "TillAccountId", "dbo.GLAccounts");
            DropIndex("dbo.Tellers", new[] { "User_Id" });
            DropIndex("dbo.Tellers", new[] { "TillAccountId" });
            DropTable("dbo.Tellers");
        }
    }
}
