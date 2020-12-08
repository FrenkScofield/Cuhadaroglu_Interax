using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSiteService.Migrations
{
    public partial class dbsdfsdfsdfsd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecContentValue_SpecId",
                table: "SpecContentValue");

            migrationBuilder.CreateIndex(
                name: "IX_SpecContentValue_SpecId",
                table: "SpecContentValue",
                column: "SpecId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecContentValue_SpecId",
                table: "SpecContentValue");

            migrationBuilder.CreateIndex(
                name: "IX_SpecContentValue_SpecId",
                table: "SpecContentValue",
                column: "SpecId",
                unique: true);
        }
    }
}
