using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class removingUniqueConstraintForApartmentNumberFromApartmentUnitsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApartmentUnits_ApartmentNumber",
                table: "ApartmentUnits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApartmentUnits_ApartmentNumber",
                table: "ApartmentUnits",
                column: "ApartmentNumber",
                unique: true);
        }
    }
}
