using Microsoft.EntityFrameworkCore.Migrations;

namespace InvoiceWebApp.Data.Migrations {

    public partial class UniqueKeyMigration : Migration {

        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "Debtors",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "BankAccount",
                table: "Debtors",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "RegNumber",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FinancialNumber",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "EUFinancialNumber",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "BankAccount",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_BankAccount",
                table: "Debtors",
                column: "BankAccount");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_IdNumber",
                table: "Debtors",
                column: "IdNumber");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_BankAccount2",
                table: "Company",
                column: "BankAccount");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_EUFinancialNumber",
                table: "Company",
                column: "EUFinancialNumber");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_FinancialNumber",
                table: "Company",
                column: "FinancialNumber");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_RegNumber",
                table: "Company",
                column: "RegNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_Name",
                table: "Products");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_BankAccount",
                table: "Debtors");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_IdNumber",
                table: "Debtors");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_BankAccount",
                table: "Company");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_EUFinancialNumber",
                table: "Company");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_FinancialNumber",
                table: "Company");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_RegNumber",
                table: "Company");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "Debtors",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "BankAccount",
                table: "Debtors",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "RegNumber",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FinancialNumber",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "EUFinancialNumber",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "BankAccount",
                table: "Company",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}