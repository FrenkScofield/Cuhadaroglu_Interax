using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSiteService.Migrations
{
    public partial class up10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "BIMFileId",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CadDataId",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicalDocumentId",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicalPropertyId",
                table: "Documents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_BIMFileId",
                table: "Documents",
                column: "BIMFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CadDataId",
                table: "Documents",
                column: "CadDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TechnicalDocumentId",
                table: "Documents",
                column: "TechnicalDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TechnicalPropertyId",
                table: "Documents",
                column: "TechnicalPropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ContentPage_BIMFileId",
                table: "Documents",
                column: "BIMFileId",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ContentPage_CadDataId",
                table: "Documents",
                column: "CadDataId",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ContentPage_TechnicalDocumentId",
                table: "Documents",
                column: "TechnicalDocumentId",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ContentPage_TechnicalPropertyId",
                table: "Documents",
                column: "TechnicalPropertyId",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ContentPage_BIMFileId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ContentPage_CadDataId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ContentPage_TechnicalDocumentId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ContentPage_TechnicalPropertyId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_BIMFileId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CadDataId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_TechnicalDocumentId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_TechnicalPropertyId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BIMFileId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CadDataId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "TechnicalDocumentId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "TechnicalPropertyId",
                table: "Documents");

            migrationBuilder.AddColumn<int>(
                name: "ContentPageId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentPageId1",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentPageId2",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentPageId3",
                table: "Documents",
                type: "int",
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
    }
}
