using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class addingStatusToMaintenanceFeeAndSpecificFeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ResidentsSpecificDebts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MaintenanceFees",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ResidentsSpecificDebts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MaintenanceFees");
        }
    }
}
