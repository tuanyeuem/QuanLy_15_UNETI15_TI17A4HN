using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLy_15_UNETI15_TI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class TaoCSDLDayDuKhachSan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.MaDichVu);
                    table.CheckConstraint("CK_DichVu_DonGia", "[DonGia] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CCCD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QuocTich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.MaKhachHang);
                    table.ForeignKey(
                        name: "FK_KhachHangs_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phongs",
                columns: table => new
                {
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaLoaiPhong = table.Column<int>(type: "int", nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    HuongPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TienNghi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayBaoTri = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChuBaoTri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phongs", x => x.MaPhong);
                    table.CheckConstraint("CK_Phong_DonGia", "[DonGia] >= 0");
                    table.CheckConstraint("CK_Phong_Tang", "[Tang] > 0");
                    table.CheckConstraint("CK_Phong_TrangThai", "[TrangThai] IN ('Trong','DaDat','DangSuDung','BaoTri','NgungKinhDoanh')");
                    table.ForeignKey(
                        name: "FK_Phongs_LoaiPhongs_MaLoaiPhong",
                        column: x => x.MaLoaiPhong,
                        principalTable: "LoaiPhongs",
                        principalColumn: "MaLoaiPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatPhongs",
                columns: table => new
                {
                    MaDatPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaNhanVienTiepNhan = table.Column<int>(type: "int", nullable: true),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTraDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoNguoi = table.Column<int>(type: "int", nullable: false),
                    TienCoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatPhongs", x => x.MaDatPhong);
                    table.CheckConstraint("CK_DatPhong_Ngay", "[NgayTraDuKien] > [NgayNhan]");
                    table.CheckConstraint("CK_DatPhong_SoNguoi", "[SoNguoi] > 0");
                    table.CheckConstraint("CK_DatPhong_TienCoc", "[TienCoc] >= 0");
                    table.CheckConstraint("CK_DatPhong_TrangThai", "[TrangThai] IN ('ChoXuLy','DangXuLy','HoanThanh','Huy','TuChoi')");
                    table.ForeignKey(
                        name: "FK_DatPhongs_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatPhongs_TaiKhoans_MaNhanVienTiepNhan",
                        column: x => x.MaNhanVienTiepNhan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDatPhongs",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatPhong = table.Column<int>(type: "int", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoNgay = table.Column<int>(type: "int", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDatPhongs", x => x.MaChiTiet);
                    table.CheckConstraint("CK_ChiTietDatPhong_Ngay", "[NgayTra] > [NgayNhan]");
                    table.CheckConstraint("CK_ChiTietDatPhong_Tien", "[DonGia] >= 0 AND [SoNgay] > 0 AND [ThanhTien] >= 0");
                    table.CheckConstraint("CK_ChiTietDatPhong_TrangThai", "[TrangThai] IN ('DaDat','DangSuDung','DaTraPhong','Huy')");
                    table.ForeignKey(
                        name: "FK_ChiTietDatPhongs_DatPhongs_MaDatPhong",
                        column: x => x.MaDatPhong,
                        principalTable: "DatPhongs",
                        principalColumn: "MaDatPhong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietDatPhongs_Phongs_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phongs",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuDungDichVus",
                columns: table => new
                {
                    MaSuDungDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChiTiet = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgaySuDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuDungDichVus", x => x.MaSuDungDichVu);
                    table.CheckConstraint("CK_SuDungDichVu_SoLuong", "[SoLuong] > 0");
                    table.CheckConstraint("CK_SuDungDichVu_Tien", "[DonGia] >= 0 AND [ThanhTien] >= 0");
                    table.ForeignKey(
                        name: "FK_SuDungDichVus_ChiTietDatPhongs_MaChiTiet",
                        column: x => x.MaChiTiet,
                        principalTable: "ChiTietDatPhongs",
                        principalColumn: "MaChiTiet",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuDungDichVus_DichVus_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVus",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDatPhongs_MaDatPhong_MaPhong",
                table: "ChiTietDatPhongs",
                columns: new[] { "MaDatPhong", "MaPhong" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDatPhongs_MaPhong_NgayNhan_NgayTra",
                table: "ChiTietDatPhongs",
                columns: new[] { "MaPhong", "NgayNhan", "NgayTra" });

            migrationBuilder.CreateIndex(
                name: "IX_DatPhongs_MaKhachHang_NgayDat",
                table: "DatPhongs",
                columns: new[] { "MaKhachHang", "NgayDat" });

            migrationBuilder.CreateIndex(
                name: "IX_DatPhongs_MaNhanVienTiepNhan",
                table: "DatPhongs",
                column: "MaNhanVienTiepNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DatPhongs_TrangThai_NgayNhan",
                table: "DatPhongs",
                columns: new[] { "TrangThai", "NgayNhan" });

            migrationBuilder.CreateIndex(
                name: "IX_DichVus_TenDichVu",
                table: "DichVus",
                column: "TenDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_CCCD",
                table: "KhachHangs",
                column: "CCCD",
                unique: true,
                filter: "[CCCD] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_MaTaiKhoan",
                table: "KhachHangs",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

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
                name: "IX_SuDungDichVus_MaChiTiet",
                table: "SuDungDichVus",
                column: "MaChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVus_MaDichVu",
                table: "SuDungDichVus",
                column: "MaDichVu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuDungDichVus");

            migrationBuilder.DropTable(
                name: "ChiTietDatPhongs");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "DatPhongs");

            migrationBuilder.DropTable(
                name: "Phongs");

            migrationBuilder.DropTable(
                name: "KhachHangs");
        }
    }
}
