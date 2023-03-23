using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardApp.Migrations
{
    /// <inheritdoc />
    public partial class AddClickCountToSurfboards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "Board",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "Board");
        }
    }
}
