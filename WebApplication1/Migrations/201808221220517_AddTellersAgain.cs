namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTellersAgain : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tellers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        
                        TillAccount_Id = c.Int(nullable: false),
                        IsAssigned = c.Boolean(nullable: false),
                        
                        UserTeller_Id = c.String(nullable:false,maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.TillAccount_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserTeller_Id)
                .Index(t => t.TillAccount_Id)
                .Index(t => t.UserTeller_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tellers", "UserTeller_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tellers", "TillAccount_Id1", "dbo.GLAccounts");
            DropIndex("dbo.Tellers", new[] { "UserTeller_Id1" });
            DropIndex("dbo.Tellers", new[] { "TillAccount_Id1" });
            DropTable("dbo.Tellers");
        }
    }
}
