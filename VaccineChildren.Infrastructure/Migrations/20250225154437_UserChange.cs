using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccineChildren.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email_verification_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "token_expiry",
                table: "users");

            migrationBuilder.AlterColumn<bool>(
                name: "is_verified",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "unit",
                table: "packages",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "min_age",
                table: "packages",
                newName: "MinAge");

            migrationBuilder.RenameColumn(
                name: "max_age",
                table: "packages",
                newName: "MaxAge");

            migrationBuilder.AlterColumn<bool>(
                name: "is_verified",
                table: "users",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "email_verification_token",
                table: "users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "token_expiry",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
