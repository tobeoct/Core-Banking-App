namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsAssignedToGLAccounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GLAccounts", "IsAssigned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GLAccounts", "IsAssigned");
        }
    }
}
