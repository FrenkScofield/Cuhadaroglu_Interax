using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSiteService.Migrations
{
    public partial class dbsdfsdfsdfsdsdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecContentValue_Lang_LangId",
                table: "SpecContentValue");

            migrationBuilder.AlterColumn<int>(
                name: "LangId",
                table: "SpecContentValue",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecContentValue_Lang_LangId",
                table: "SpecContentValue",
                column: "LangId",
                principalTable: "Lang",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecContentValue_Lang_LangId",
                table: "SpecContentValue");

            migrationBuilder.AlterColumn<int>(
                name: "LangId",
                table: "SpecContentValue",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecContentValue_Lang_LangId",
                table: "SpecContentValue",
                column: "LangId",
                principalTable: "Lang",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
