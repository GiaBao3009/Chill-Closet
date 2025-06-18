using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chill_Closet.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherIssuedFlagToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLateVoucherIssued",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLateVoucherIssued",
                table: "Orders");
        }
    }
}
