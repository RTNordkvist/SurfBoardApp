using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfBoardApp.Migrations
{
    /// <inheritdoc />
    public partial class FunctionalityForNonMembersAddedToBoardAndBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_AspNetUsers_CustomerId",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Booking",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "NonUserEmail",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MembersOnly",
                table: "Board",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_AspNetUsers_CustomerId",
                table: "Booking",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_AspNetUsers_CustomerId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "NonUserEmail",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "MembersOnly",
                table: "Board");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Booking",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_AspNetUsers_CustomerId",
                table: "Booking",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
