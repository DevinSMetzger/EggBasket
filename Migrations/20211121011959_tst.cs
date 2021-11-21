using Microsoft.EntityFrameworkCore.Migrations;

namespace EggBasket.Migrations
{
    public partial class tst : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<string>(
                name: "userIds",
                table: "Credential",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userIds",
                table: "Credential");

            migrationBuilder.CreateTable(
                name: "Wrapper",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CredentialID = table.Column<int>(type: "int", nullable: true),
                    userid = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
    }
}
