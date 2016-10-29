namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BankAccounts", "UserId", c => c.String());
            AlterColumn("dbo.Cards", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cards", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.BankAccounts", "UserId", c => c.Int(nullable: false));
        }
    }
}
