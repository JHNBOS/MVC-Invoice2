using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using InvoiceWebApp.Data;

namespace InvoiceWebApp.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InvoiceWebApp.Models.Admin", b =>
                {
                    b.Property<int>("AdminID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("AdminID");

                    b.HasAlternateKey("Email")
                        .HasName("AlternateKey_AdminEmail");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("AccountType");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int>("DebtorID");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("DebtorID");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.AppSettings", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("BankAccountNumber");

                    b.Property<string>("BankName");

                    b.Property<string>("City");

                    b.Property<string>("CompanyName");

                    b.Property<string>("Country");

                    b.Property<string>("EUFinancialNumber");

                    b.Property<string>("Email");

                    b.Property<string>("FinancialNumber");

                    b.Property<string>("Logo");

                    b.Property<string>("Password");

                    b.Property<string>("Phone");

                    b.Property<int>("Port");

                    b.Property<string>("PostalCode");

                    b.Property<string>("Prefix");

                    b.Property<string>("RegNumber");

                    b.Property<string>("SMTP");

                    b.Property<bool>("UseLogo");

                    b.Property<string>("Website");

                    b.HasKey("ID");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.Company", b =>
                {
                    b.Property<int>("CompanyID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<string>("BankAccount")
                        .IsRequired();

                    b.Property<string>("BankName")
                        .IsRequired();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("CompanyName")
                        .IsRequired();

                    b.Property<string>("Country")
                        .IsRequired();

                    b.Property<string>("EUFinancialNumber")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FinancialNumber")
                        .IsRequired();

                    b.Property<string>("Phone");

                    b.Property<string>("PostalCode")
                        .IsRequired();

                    b.Property<string>("RegNumber")
                        .IsRequired();

                    b.HasKey("CompanyID");

                    b.HasAlternateKey("BankAccount")
                        .HasName("AlternateKey_BankAccount");


                    b.HasAlternateKey("EUFinancialNumber")
                        .HasName("AlternateKey_EUFinancialNumber");


                    b.HasAlternateKey("FinancialNumber")
                        .HasName("AlternateKey_FinancialNumber");


                    b.HasAlternateKey("RegNumber")
                        .HasName("AlternateKey_RegNumber");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.Debtor", b =>
                {
                    b.Property<int>("DebtorID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<string>("BankAccount")
                        .IsRequired();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("Country")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("IdNumber")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Phone");

                    b.Property<string>("PostalCode")
                        .IsRequired();

                    b.HasKey("DebtorID");

                    b.HasAlternateKey("BankAccount")
                        .HasName("AlternateKey_BankAccount");


                    b.HasAlternateKey("IdNumber")
                        .HasName("AlternateKey_IdNumber");

                    b.ToTable("Debtors");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.Invoice", b =>
                {
                    b.Property<int>("InvoiceNumber")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyID");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int?>("DebtorID");

                    b.Property<DateTime>("ExpirationDate");

                    b.Property<bool>("Paid");

                    b.Property<decimal>("Total");

                    b.Property<string>("Type");

                    b.HasKey("InvoiceNumber");

                    b.HasIndex("CompanyID");

                    b.HasIndex("DebtorID");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.InvoiceItem", b =>
                {
                    b.Property<int>("ItemID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.Property<int>("InvoiceNumber");

                    b.Property<int>("ProductID");

                    b.HasKey("ItemID");

                    b.HasIndex("InvoiceNumber");

                    b.HasIndex("ProductID");

                    b.ToTable("InvoiceItems");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.Product", b =>
                {
                    b.Property<int>("ProductID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<decimal>("Price");

                    b.Property<int>("VAT");

                    b.HasKey("ProductID");

                    b.HasAlternateKey("Name")
                        .HasName("AlternateKey_Name");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountType");

                    b.Property<int>("DebtorID");

                    b.Property<string>("Email");

                    b.Property<string>("Password");

                    b.HasKey("ID");

                    b.HasIndex("DebtorID")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.ApplicationUser", b =>
                {
                    b.HasOne("InvoiceWebApp.Models.Debtor", "Debtor")
                        .WithMany()
                        .HasForeignKey("DebtorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InvoiceWebApp.Models.Invoice", b =>
                {
                    b.HasOne("InvoiceWebApp.Models.Company", "Company")
                        .WithMany("Invoices")
                        .HasForeignKey("CompanyID");

                    b.HasOne("InvoiceWebApp.Models.Debtor", "Debtor")
                        .WithMany("Invoices")
                        .HasForeignKey("DebtorID");
                });

            modelBuilder.Entity("InvoiceWebApp.Models.InvoiceItem", b =>
                {
                    b.HasOne("InvoiceWebApp.Models.Invoice", "Invoice")
                        .WithMany("InvoiceItems")
                        .HasForeignKey("InvoiceNumber")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InvoiceWebApp.Models.Product", "Product")
                        .WithMany("InvoiceItems")
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InvoiceWebApp.Models.User", b =>
                {
                    b.HasOne("InvoiceWebApp.Models.Debtor", "Debtor")
                        .WithOne("User")
                        .HasForeignKey("InvoiceWebApp.Models.User", "DebtorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("InvoiceWebApp.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("InvoiceWebApp.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InvoiceWebApp.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
