using Microsoft.EntityFrameworkCore.Migrations;

namespace EggBasket.Migrations
{
    public partial class wrapper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "Credential");

            migrationBuilder.CreateTable(
                name: "Wrapper",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredentialID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wrapper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wrapper_Credential_CredentialID",
                        column: x => x.CredentialID,
                        principalTable: "Credential",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wrapper_CredentialID",
                table: "Wrapper",
                column: "CredentialID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wrapper");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Credential",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
