using Microsoft.EntityFrameworkCore.Migrations;

namespace EggBasket.Data.Migrations
{
    public partial class Addedkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PemPrivateKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PemPublicKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PemPrivateKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PemPublicKey",
                table: "AspNetUsers");
        }
    }
}
