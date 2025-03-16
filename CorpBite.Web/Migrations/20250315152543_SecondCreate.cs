using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorpBite.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PreparationEndTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreparationStartTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PreparationTimeExtension",
                table: "Orders",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreparationEndTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PreparationStartTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PreparationTimeExtension",
                table: "Orders");
        }
    }
}
