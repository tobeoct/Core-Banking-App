namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTellers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tellers", "TillAccount_Id1", "dbo.GLAccounts");
            DropForeignKey("dbo.Tellers", "UserTeller_Id1", "dbo.AspNetUsers");
            DropIndex("dbo.Tellers", new[] { "TillAccount_Id1" });
            DropIndex("dbo.Tellers", new[] { "UserTeller_Id1" });
            DropTable("dbo.Tellers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Tellers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserTeller_Id = c.Int(nullable: false),
                        TillAccount_Id = c.Int(nullable: false),
                        IsAssigned = c.Boolean(nullable: false),
                        TillAccount_Id1 = c.Int(),
                        UserTeller_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Tellers", "UserTeller_Id1");
            CreateIndex("dbo.Tellers", "TillAccount_Id1");
            AddForeignKey("dbo.Tellers", "UserTeller_Id1", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Tellers", "TillAccount_Id1", "dbo.GLAccounts", "Id");
        }
    }
}
