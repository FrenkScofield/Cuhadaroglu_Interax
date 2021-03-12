using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSiteService.Migrations
{
    public partial class v111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Linkedin",
                table: "SiteConfig",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Linkedin",
                table: "SiteConfig");
        }
    }
}
