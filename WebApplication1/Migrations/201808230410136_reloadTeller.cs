namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reloadTeller : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tellers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserTellerId = c.String(maxLength: 128),
                        TillAccountId = c.Int(nullable: false),
                        IsAssigned = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.TillAccountId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserTellerId)
                .Index(t => t.UserTellerId)
                .Index(t => t.TillAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tellers", "UserTellerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tellers", "TillAccountId", "dbo.GLAccounts");
            DropIndex("dbo.Tellers", new[] { "TillAccountId" });
            DropIndex("dbo.Tellers", new[] { "UserTellerId" });
            DropTable("dbo.Tellers");
        }
    }
}
