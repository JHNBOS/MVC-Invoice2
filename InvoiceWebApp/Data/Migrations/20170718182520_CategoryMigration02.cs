using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InvoiceWebApp.Data.Migrations
{
    public partial class CategoryMigration02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryID1",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID1",
                table: "Products",
                column: "CategoryID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryID",
                table: "Products",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryID1",
                table: "Products",
                column: "CategoryID1",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryID",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryID1",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryID1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryID1",
                table: "Products");
        }
    }
}
