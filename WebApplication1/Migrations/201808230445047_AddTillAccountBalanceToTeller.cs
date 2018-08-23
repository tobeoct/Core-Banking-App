namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTillAccountBalanceToTeller : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tellers", "TillAccountBalance", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tellers", "TillAccountBalance");
        }
    }
}
