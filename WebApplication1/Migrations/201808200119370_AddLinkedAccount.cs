namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLinkedAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanDetails", "linkedCustomerAccountId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoanDetails", "linkedCustomerAccountId");
        }
    }
}
