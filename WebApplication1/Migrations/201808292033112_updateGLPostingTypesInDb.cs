namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateGLPostingTypesInDb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GLPostings", "DebitAmount", c => c.Single(nullable: false));
            AlterColumn("dbo.GLPostings", "CreditAmount", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GLPostings", "CreditAmount", c => c.Long(nullable: false));
            AlterColumn("dbo.GLPostings", "DebitAmount", c => c.Long(nullable: false));
        }
    }
}
