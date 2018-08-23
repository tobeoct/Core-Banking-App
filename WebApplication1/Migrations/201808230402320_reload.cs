namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reload : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tellers", "TillAccount_Id", "dbo.GLAccounts");
            DropForeignKey("dbo.Tellers", "UserTeller_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Tellers", new[] { "TillAccount_Id" });
            DropIndex("dbo.Tellers", new[] { "UserTeller_Id" });
            DropTable("dbo.Tellers");
        }
        
        public override void Down()
        {
            CreateTable(
                    "dbo.Tellers",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        
                        IsAssigned = c.Boolean(nullable: false),
                        TillAccount_Id = c.Int(),
                        UserTeller_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);

            CreateIndex("dbo.Tellers", "UserTeller_Id");
            CreateIndex("dbo.Tellers", "TillAccount_Id");
            AddForeignKey("dbo.Tellers", "UserTeller_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Tellers", "TillAccount_Id", "dbo.GLAccounts", "Id");
        }
    }
}
