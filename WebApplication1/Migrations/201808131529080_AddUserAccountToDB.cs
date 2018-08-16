namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAccountToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 1000),
                        BranchId = c.Int(nullable: false),
                        Email = c.String(),
                        Username = c.String(maxLength: 1000),
                        PhoneNumber = c.String(),
                        Password = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserAccounts", "BranchId", "dbo.Branches");
            DropIndex("dbo.UserAccounts", new[] { "BranchId" });
            DropTable("dbo.UserAccounts");
        }
    }
}
