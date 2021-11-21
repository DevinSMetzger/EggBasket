using Microsoft.EntityFrameworkCore.Migrations;

namespace EggBasket.Migrations
{
    public partial class werwe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Credential",
                table: "Credential");

            migrationBuilder.RenameTable(
                name: "Credential",
                newName: "CredentialAccess");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CredentialAccess",
                table: "CredentialAccess",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "CredentialAcess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    credential = table.Column<int>(type: "int", nullable: false),
                    userid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialAcess", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CredentialAcess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CredentialAccess",
                table: "CredentialAccess");

            migrationBuilder.RenameTable(
                name: "CredentialAccess",
                newName: "Credential");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Credential",
                table: "Credential",
                column: "ID");
        }
    }
}
