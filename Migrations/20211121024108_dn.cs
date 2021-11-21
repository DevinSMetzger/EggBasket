using Microsoft.EntityFrameworkCore.Migrations;

namespace EggBasket.Migrations
{
    public partial class dn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userIds",
                table: "Credential");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userIds",
                table: "Credential",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
