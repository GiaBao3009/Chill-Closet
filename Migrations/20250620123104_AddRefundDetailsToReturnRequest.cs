using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chill_Closet.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundDetailsToReturnRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankAccountName",
                table: "ReturnRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "ReturnRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "ReturnRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MomoPhoneNumber",
                table: "ReturnRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefundMethod",
                table: "ReturnRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountName",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "MomoPhoneNumber",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "RefundMethod",
                table: "ReturnRequests");
        }
    }
}
