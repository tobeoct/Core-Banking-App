namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateCategories : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Categories (Name) VALUES('Asset')");
            Sql("INSERT INTO Categories (Name) VALUES('Liability')");
            Sql("INSERT INTO Categories (Name) VALUES('Capital')");
            Sql("INSERT INTO Categories (Name) VALUES('Income')");
            Sql("INSERT INTO Categories (Name) VALUES('Expense')");
        }
        
        public override void Down()
        {
        }
    }
}
