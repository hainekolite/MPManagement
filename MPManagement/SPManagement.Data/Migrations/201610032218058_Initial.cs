namespace SPManagement.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cartuchos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumeroDeCartucho = c.String(),
                        FechaEntrada = c.DateTime(nullable: false),
                        FechaSalida = c.DateTime(nullable: false),
                        FechaRecepcion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        RefrigeradorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Refrigeradores", t => t.RefrigeradorId, cascadeDelete: true)
                .Index(t => t.RefrigeradorId);
            
            CreateTable(
                "dbo.Refrigeradores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumeroDeRefrigerador = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.NumeroDeRefrigerador, unique: true, name: "Index");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cartuchos", "RefrigeradorId", "dbo.Refrigeradores");
            DropIndex("dbo.Refrigeradores", "Index");
            DropIndex("dbo.Cartuchos", new[] { "RefrigeradorId" });
            DropTable("dbo.Refrigeradores");
            DropTable("dbo.Cartuchos");
        }
    }
}
