namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mails",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        email = c.String(),
                        name = c.String(),
                        phone = c.String(),
                        message = c.String(),
                        time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            
            
        }
        
        public override void Down()
        {
            
        }
    }
}
