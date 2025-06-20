using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chill_Closet.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Vouchers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_ApplicationUserId",
                table: "Vouchers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_AspNetUsers_ApplicationUserId",
                table: "Vouchers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_AspNetUsers_ApplicationUserId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_ApplicationUserId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Vouchers");
        }
    }
}
