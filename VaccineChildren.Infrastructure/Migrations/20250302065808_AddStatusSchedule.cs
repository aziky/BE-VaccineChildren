using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccineChildren.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "pre_vaccine_checkup",
                table: "schedule",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "schedule",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
