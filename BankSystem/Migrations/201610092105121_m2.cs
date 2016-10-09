namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "phone", c => c.String());
            AddColumn("dbo.AspNetUsers", "adress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "adress");
            DropColumn("dbo.AspNetUsers", "phone");
        }
    }
}
