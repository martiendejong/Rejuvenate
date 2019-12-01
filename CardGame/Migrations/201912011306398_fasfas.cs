namespace CardGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fasfas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Game_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.Game_Id)
                .Index(t => t.Game_Id);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "Game_Id", "dbo.Games");
            DropIndex("dbo.Players", new[] { "Game_Id" });
            DropTable("dbo.Games");
            DropTable("dbo.Players");
        }
    }
}
