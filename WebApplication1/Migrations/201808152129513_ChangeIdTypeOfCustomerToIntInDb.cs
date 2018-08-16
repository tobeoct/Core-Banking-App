namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeIdTypeOfCustomerToIntInDb : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Customers");
            AlterColumn("dbo.Customers", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Customers");
            AlterColumn("dbo.Customers", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Customers", "Id");
        }
    }
}
