using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSiteService.Migrations
{
    public partial class dbdffdsfdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsIletisimIzın",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsKvk",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIletisimIzın",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsKvk",
                table: "User");
        }
    }
}
