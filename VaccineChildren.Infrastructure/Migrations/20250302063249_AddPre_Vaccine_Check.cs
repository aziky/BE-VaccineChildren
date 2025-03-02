using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaccineChildren.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPre_Vaccine_Check : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pre_vaccine_checkup",
                table: "schedule",
                type: "text",
                nullable: true,
                defaultValue: "");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
