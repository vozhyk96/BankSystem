namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mails", "time", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mails", "time");
        }
    }
}
