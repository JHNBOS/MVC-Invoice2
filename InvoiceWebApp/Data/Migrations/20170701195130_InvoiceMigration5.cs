using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvoiceWebApp.Data.Migrations
{
    public partial class InvoiceMigration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Company_CompanyID1",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CompanyID1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CompanyID1",
                table: "Invoices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyID1",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CompanyID1",
                table: "Invoices",
                column: "CompanyID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Company_CompanyID1",
                table: "Invoices",
                column: "CompanyID1",
                principalTable: "Company",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
