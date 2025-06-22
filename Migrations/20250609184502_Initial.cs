using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chill_Closet.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // === CÁC BẢNG IDENTITY ĐÃ TỒN TẠI, COMMENT OUT ĐỂ BỎ QUA VIỆC TẠO LẠI ===

            // migrationBuilder.CreateTable(
            //     name: "AspNetRoles", ... );

            // migrationBuilder.CreateTable(
            //     name: "AspNetUsers", ... );

            // migrationBuilder.CreateTable(
            //     name: "AspNetRoleClaims", ... );

            // migrationBuilder.CreateTable(
            //     name: "AspNetUserClaims", ... );

            // migrationBuilder.CreateTable(
            //     name: "AspNetUserLogins", ... );

            // migrationBuilder.CreateTable(
            //     name: "AspNetUserRoles", ... );

            // migrationBuilder.CreateTable(
            //     name: "AspNetUserTokens", ... );


            // === GIỮ LẠI VIỆC TẠO CÁC BẢNG MỚI CỦA ỨNG DỤNG ===

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // === COMMENT OUT CÁC INDEX ĐÃ TỒN TẠI CỦA IDENTITY ===

            // migrationBuilder.CreateIndex(
            //     name: "IX_AspNetRoleClaims_RoleId", ...);

            // migrationBuilder.CreateIndex(
            //     name: "RoleNameIndex", ...);

            // ... Tương tự cho các Index AspNet... khác ...

            // === GIỮ LẠI VIỆC TẠO CÁC INDEX MỚI CHO ỨNG DỤNG ===
            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Giữ nguyên phương thức Down để có thể hoàn tác nếu cần
            migrationBuilder.DropTable(name: "AspNetRoleClaims");
            migrationBuilder.DropTable(name: "AspNetUserClaims");
            migrationBuilder.DropTable(name: "AspNetUserLogins");
            migrationBuilder.DropTable(name: "AspNetUserRoles");
            migrationBuilder.DropTable(name: "AspNetUserTokens");
            migrationBuilder.DropTable(name: "Orders");
            migrationBuilder.DropTable(name: "Products");
            migrationBuilder.DropTable(name: "AspNetRoles");
            migrationBuilder.DropTable(name: "AspNetUsers");
            migrationBuilder.DropTable(name: "Categories");
        }
    }
}