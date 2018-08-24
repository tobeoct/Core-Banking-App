namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fghje : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tellers", "TillAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.Tellers", "UserTellerId", "dbo.AspNetUsers");
            DropIndex("dbo.Tellers", new[] { "UserTellerId" });
            DropIndex("dbo.Tellers", new[] { "TillAccountId" });
            DropTable("dbo.Tellers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Tellers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserTellerId = c.String(maxLength: 128),
                        TillAccountId = c.Int(nullable: false),
                        TillAccountBalance = c.Long(nullable: false),
                        IsAssigned = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Tellers", "TillAccountId");
            CreateIndex("dbo.Tellers", "UserTellerId");
            AddForeignKey("dbo.Tellers", "UserTellerId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Tellers", "TillAccountId", "dbo.GLAccounts", "Id", cascadeDelete: true);
        }
    }
}
