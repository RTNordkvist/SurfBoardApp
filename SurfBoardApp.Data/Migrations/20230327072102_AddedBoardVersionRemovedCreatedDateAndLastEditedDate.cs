using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedBoardVersionRemovedCreatedDateAndLastEditedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "LastEditedDate",
                table: "Board");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Board",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Board");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Board",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedDate",
                table: "Board",
                type: "datetime2",
                nullable: true);
        }
    }
}
