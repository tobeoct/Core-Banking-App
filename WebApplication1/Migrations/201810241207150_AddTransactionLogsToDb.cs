namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransactionLogsToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionLogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CardPan = c.String(),
                        MTI = c.String(),
                        Amount = c.Single(nullable: false),
                        STAN = c.String(),
                        TransactionDate = c.DateTime(nullable: false),
                        Account1 = c.String(),
                        Account2 = c.String(),
                        ResponseCode = c.String(),
                        TypeOfEntry = c.String(),
                        Narration = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TransactionLogs");
        }
    }
}
