namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Cards", "money");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cards", "money", c => c.Int(nullable: false));
        }
    }
}
