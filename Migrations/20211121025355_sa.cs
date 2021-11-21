using Microsoft.EntityFrameworkCore.Migrations;

namespace EggBasket.Migrations
{
    public partial class sa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CredentialAcess");

            migrationBuilder.DropColumn(
                name: "company",
                table: "CredentialAccess");

            migrationBuilder.DropColumn(
                name: "password",
                table: "CredentialAccess");

            migrationBuilder.DropColumn(
                name: "personal",
                table: "CredentialAccess");

            migrationBuilder.DropColumn(
                name: "roleID",
                table: "CredentialAccess");

            migrationBuilder.DropColumn(
                name: "secureNote",
                table: "CredentialAccess");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CredentialAccess",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "CredentialAccess",
                newName: "userid");

            migrationBuilder.AddColumn<int>(
                name: "credential",
                table: "CredentialAccess",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Credential",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    secureNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    roleID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    personal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credential", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credential");

            migrationBuilder.DropColumn(
                name: "credential",
                table: "CredentialAccess");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CredentialAccess",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "CredentialAccess",
                newName: "username");

            migrationBuilder.AddColumn<string>(
                name: "company",
                table: "CredentialAccess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "CredentialAccess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "personal",
                table: "CredentialAccess",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "roleID",
                table: "CredentialAccess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secureNote",
                table: "CredentialAccess",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
