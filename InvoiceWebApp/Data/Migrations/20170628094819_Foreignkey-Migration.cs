using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvoiceWebApp.Data.Migrations
{
    public partial class ForeignkeyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DebtorID",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DebtorID",
                table: "Users",
                column: "DebtorID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DebtorID",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DebtorID",
                table: "Users",
                column: "DebtorID");
        }
    }
}
