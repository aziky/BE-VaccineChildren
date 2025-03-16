using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccineChildren.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFkVacineToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "vaccine_manufactures_vaccine_id_key",
                table: "vaccine_manufactures");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "schedule",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "schedule_date",
                table: "schedule",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "schedule",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "actual_date",
                table: "schedule",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "vaccine_manufactures_vaccine_id_key",
                table: "vaccine_manufactures",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_vaccine_id",
                table: "schedule",
                column: "vaccine_id");

            migrationBuilder.AddForeignKey(
                name: "schedule_vaccine_id_fkey",
                table: "schedule",
                column: "vaccine_id",
                principalTable: "vaccine",
                principalColumn: "vaccine_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "schedule_vaccine_id_fkey",
                table: "schedule");

            migrationBuilder.DropIndex(
                name: "vaccine_manufactures_vaccine_id_key",
                table: "vaccine_manufactures");

            migrationBuilder.DropIndex(
                name: "IX_schedule_vaccine_id",
                table: "schedule");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "schedule",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "schedule_date",
                table: "schedule",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "schedule",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "actual_date",
                table: "schedule",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "vaccine_manufactures_vaccine_id_key",
                table: "vaccine_manufactures",
                column: "vaccine_id",
                unique: true);
        }
    }
}
