using Microsoft.EntityFrameworkCore.Migrations;

namespace InvoiceWebApp.Data.Migrations {

    public partial class NullableMigration : Migration {

        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Company_CompanyID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Debtors_DebtorID",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "DebtorID",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Company_CompanyID",
                table: "Invoices",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Debtors_DebtorID",
                table: "Invoices",
                column: "DebtorID",
                principalTable: "Debtors",
                principalColumn: "DebtorID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Company_CompanyID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Debtors_DebtorID",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "DebtorID",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Company_CompanyID",
                table: "Invoices",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Debtors_DebtorID",
                table: "Invoices",
                column: "DebtorID",
                principalTable: "Debtors",
                principalColumn: "DebtorID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}