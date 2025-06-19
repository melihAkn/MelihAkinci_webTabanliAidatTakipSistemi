using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role",
                table: "UserRoles",
                newName: "Role");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Role" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "ApartmentManager" },
                    { 3, "ApartmentResident" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "UserRoles",
                newName: "role");
        }
    }
}
