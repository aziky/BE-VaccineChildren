using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccineChildren.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVaccineTypeToVaccineId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vaccine_type",
                table: "schedule");

            migrationBuilder.AddColumn<Guid>(
                name: "vaccine_id",
                table: "schedule",
                type: "uuid",
                maxLength: 255,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vaccine_id",
                table: "schedule");

            migrationBuilder.AddColumn<string>(
                name: "vaccine_type",
                table: "schedule",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
