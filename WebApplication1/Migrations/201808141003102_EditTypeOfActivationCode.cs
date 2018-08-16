namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditTypeOfActivationCode : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "ActivationCode", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "ActivationCode", c => c.String());
        }
    }
}
