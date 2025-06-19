using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "ApartmentResidents",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApartmentResidents",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "ApartmentManagers",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApartmentManagers",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentUnits_ApartmentNumber",
                table: "ApartmentUnits",
                column: "ApartmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentResidents_Email",
                table: "ApartmentResidents",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentResidents_Username",
                table: "ApartmentResidents",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentManagers_Email",
                table: "ApartmentManagers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentManagers_Username",
                table: "ApartmentManagers",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApartmentUnits_ApartmentNumber",
                table: "ApartmentUnits");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentResidents_Email",
                table: "ApartmentResidents");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentResidents_Username",
                table: "ApartmentResidents");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentManagers_Email",
                table: "ApartmentManagers");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentManagers_Username",
                table: "ApartmentManagers");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "ApartmentResidents",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApartmentResidents",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "ApartmentManagers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApartmentManagers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
