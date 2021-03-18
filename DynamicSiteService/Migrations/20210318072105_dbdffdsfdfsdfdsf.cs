using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSiteService.Migrations
{
    public partial class dbdffdsfdfsdfdsf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreaDate = table.Column<DateTime>(nullable: false),
                    CreaUser = table.Column<int>(nullable: false),
                    ModUser = table.Column<int>(nullable: true),
                    ModDate = table.Column<DateTime>(nullable: true),
                    OrderNo = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<DateTime>(nullable: true),
                    IsStatus = table.Column<int>(nullable: true),
                    LoginCount = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProduct_ContentPage_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ContentPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectProduct_ContentPage_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ContentPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProduct_ProductId",
                table: "ProjectProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProduct_ProjectId",
                table: "ProjectProduct",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectProduct");
        }
    }
}
