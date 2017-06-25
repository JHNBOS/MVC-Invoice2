using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace InvoiceWebApp.Data.Migrations {

    public partial class InitialMigration01 : Migration {

        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DebtorID",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    BankAccountNumber = table.Column<string>(nullable: true),
                    BankName = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    EUFinancialNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FinancialNumber = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Port = table.Column<int>(nullable: false),
                    PostalCode = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    RegNumber = table.Column<string>(nullable: true),
                    SMTP = table.Column<string>(nullable: true),
                    UseLogo = table.Column<bool>(nullable: false),
                    Website = table.Column<string>(nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Settings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new {
                    CompanyID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: false),
                    BankAccount = table.Column<string>(nullable: false),
                    BankName = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    CompanyName = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    EUFinancialNumber = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FinancialNumber = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: false),
                    RegNumber = table.Column<string>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Company", x => x.CompanyID);
                });

            migrationBuilder.CreateTable(
                name: "Debtors",
                columns: table => new {
                    DebtorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: false),
                    BankAccount = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    IdNumber = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Debtors", x => x.DebtorID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new {
                    ProductID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    VAT = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new {
                    InvoiceNumber = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyID = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    DebtorID = table.Column<int>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    Paid = table.Column<bool>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceNumber);
                    table.ForeignKey(
                        name: "FK_Invoices_Company_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Company",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Debtors_DebtorID",
                        column: x => x.DebtorID,
                        principalTable: "Debtors",
                        principalColumn: "DebtorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountType = table.Column<string>(nullable: true),
                    DebtorID = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_Debtors_DebtorID",
                        column: x => x.DebtorID,
                        principalTable: "Debtors",
                        principalColumn: "DebtorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new {
                    ItemID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<int>(nullable: false),
                    ProductID = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_InvoiceItems", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceNumber",
                        column: x => x.InvoiceNumber,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DebtorID",
                table: "AspNetUsers",
                column: "DebtorID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CompanyID",
                table: "Invoices",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DebtorID",
                table: "Invoices",
                column: "DebtorID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceNumber",
                table: "InvoiceItems",
                column: "InvoiceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_ProductID",
                table: "InvoiceItems",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DebtorID",
                table: "Users",
                column: "DebtorID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Debtors_DebtorID",
                table: "AspNetUsers",
                column: "DebtorID",
                principalTable: "Debtors",
                principalColumn: "DebtorID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Debtors_DebtorID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Debtors");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DebtorID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DebtorID",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}