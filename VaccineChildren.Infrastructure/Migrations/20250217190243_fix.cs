using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccineChildren.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "package_vaccine_package_id_fkey",
                table: "package_vaccine");

            migrationBuilder.DropForeignKey(
                name: "package_vaccine_service_id_fkey",
                table: "package_vaccine");

            migrationBuilder.RenameColumn(
                name: "vaccine_id",
                table: "package_vaccine",
                newName: "VaccineId");

            migrationBuilder.RenameColumn(
                name: "package_id",
                table: "package_vaccine",
                newName: "PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_package_vaccine_vaccine_id",
                table: "package_vaccine",
                newName: "IX_package_vaccine_VaccineId");

            migrationBuilder.AddForeignKey(
                name: "FK_package_vaccine_packages_PackageId",
                table: "package_vaccine",
                column: "PackageId",
                principalTable: "packages",
                principalColumn: "package_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_package_vaccine_vaccine_VaccineId",
                table: "package_vaccine",
                column: "VaccineId",
                principalTable: "vaccine",
                principalColumn: "vaccine_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_package_vaccine_packages_PackageId",
                table: "package_vaccine");

            migrationBuilder.DropForeignKey(
                name: "FK_package_vaccine_vaccine_VaccineId",
                table: "package_vaccine");

            migrationBuilder.RenameColumn(
                name: "VaccineId",
                table: "package_vaccine",
                newName: "vaccine_id");

            migrationBuilder.RenameColumn(
                name: "PackageId",
                table: "package_vaccine",
                newName: "package_id");

            migrationBuilder.RenameIndex(
                name: "IX_package_vaccine_VaccineId",
                table: "package_vaccine",
                newName: "IX_package_vaccine_vaccine_id");

            migrationBuilder.AddForeignKey(
                name: "package_vaccine_package_id_fkey",
                table: "package_vaccine",
                column: "package_id",
                principalTable: "packages",
                principalColumn: "package_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "package_vaccine_service_id_fkey",
                table: "package_vaccine",
                column: "vaccine_id",
                principalTable: "vaccine",
                principalColumn: "vaccine_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
