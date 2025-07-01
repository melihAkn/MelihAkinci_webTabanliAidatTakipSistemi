using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class forResidentSpecificDebtAddingSomeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ResidentsSpecificDebts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "ResidentsSpecificDebts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "ResidentsSpecificDebts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "ResidentsSpecificDebts",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ResidentsSpecificDebts");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "ResidentsSpecificDebts");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "ResidentsSpecificDebts");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "ResidentsSpecificDebts");
        }
    }
}
