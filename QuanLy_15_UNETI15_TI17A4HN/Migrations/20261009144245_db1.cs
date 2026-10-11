using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLy_15_UNETI15_TI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class db1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiPhongs",
                columns: table => new
                {
                    MaLoaiPhong = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoNguoiToiDa = table.Column<int>(type: "int", nullable: false),
                    DonGiaNgay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhongs", x => x.MaLoaiPhong);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "Phongs",
                columns: table => new
                {
                    MaPhong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoPhong = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaLoaiPhong = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    HuongPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TienNghi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayBaoTri = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChuBaoTri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoaiPhongMaLoaiPhong = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phongs", x => x.MaPhong);
                    table.ForeignKey(
                        name: "FK_Phongs_LoaiPhongs_LoaiPhongMaLoaiPhong",
                        column: x => x.LoaiPhongMaLoaiPhong,
                        principalTable: "LoaiPhongs",
                        principalColumn: "MaLoaiPhong");
                    table.ForeignKey(
                        name: "FK_Phongs_LoaiPhongs_MaLoaiPhong",
                        column: x => x.MaLoaiPhong,
                        principalTable: "LoaiPhongs",
                        principalColumn: "MaLoaiPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoaiPhongs_TenLoai",
                table: "LoaiPhongs",
                column: "TenLoai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_LoaiPhongMaLoaiPhong",
                table: "Phongs",
                column: "LoaiPhongMaLoaiPhong");

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_MaLoaiPhong",
                table: "Phongs",
                column: "MaLoaiPhong");

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_SoPhong",
                table: "Phongs",
                column: "SoPhong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Phongs");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "LoaiPhongs");
        }
    }
}
