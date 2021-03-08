using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSiteService.Migrations
{
    public partial class documentFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContentPageId",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentPageId1",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentPageId2",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentPageId3",
                table: "Documents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ContentPageId",
                table: "Documents",
                column: "ContentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ContentPageId1",
                table: "Documents",
                column: "ContentPageId1");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ContentPageId2",
                table: "Documents",
                column: "ContentPageId2");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ContentPageId3",
                table: "Documents",
                column: "ContentPageId3");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ContentPage_ContentPageId",
                table: "Documents",
                column: "ContentPageId",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ContentPage_ContentPageId1",
                table: "Documents",
                column: "ContentPageId1",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ContentPage_ContentPageId2",
                table: "Documents",
                column: "ContentPageId2",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ContentPage_ContentPageId3",
                table: "Documents",
                column: "ContentPageId3",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ContentPage_ContentPageId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ContentPage_ContentPageId1",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ContentPage_ContentPageId2",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ContentPage_ContentPageId3",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ContentPageId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ContentPageId1",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ContentPageId2",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ContentPageId3",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ContentPageId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ContentPageId1",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ContentPageId2",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ContentPageId3",
                table: "Documents");
        }
    }
}
