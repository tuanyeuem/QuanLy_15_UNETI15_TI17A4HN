using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLy_15_UNETI15_TI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class SyncCurrentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_KhachHangs_CCCD",
                table: "KhachHangs");

            migrationBuilder.AlterColumn<string>(
                name: "CCCD",
                table: "KhachHangs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "DonViTinh",
                table: "DichVus",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "DichVus",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LyDoHuy",
                table: "DatPhongs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaPhong",
                table: "DatPhongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayNhanThucTe",
                table: "DatPhongs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTraThucTe",
                table: "DatPhongs",
                type: "datetime2",
                nullable: true);

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
                name: "IX_KhachHangs_CCCD",
                table: "KhachHangs",
                column: "CCCD",
                unique: true,
                filter: "[CCCD] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DatPhongs_MaPhong",
                table: "DatPhongs",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVus_MaChiTiet",
                table: "SuDungDichVus",
                column: "MaChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVus_MaDichVu",
                table: "SuDungDichVus",
                column: "MaDichVu");

            migrationBuilder.AddForeignKey(
                name: "FK_DatPhongs_Phongs_MaPhong",
                table: "DatPhongs",
                column: "MaPhong",
                principalTable: "Phongs",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatPhongs_Phongs_MaPhong",
                table: "DatPhongs");

            migrationBuilder.DropTable(
                name: "SuDungDichVus");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangs_CCCD",
                table: "KhachHangs");

            migrationBuilder.DropIndex(
                name: "IX_DatPhongs_MaPhong",
                table: "DatPhongs");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "LyDoHuy",
                table: "DatPhongs");

            migrationBuilder.DropColumn(
                name: "MaPhong",
                table: "DatPhongs");

            migrationBuilder.DropColumn(
                name: "NgayNhanThucTe",
                table: "DatPhongs");

            migrationBuilder.DropColumn(
                name: "NgayTraThucTe",
                table: "DatPhongs");

            migrationBuilder.AlterColumn<string>(
                name: "CCCD",
                table: "KhachHangs",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DonViTinh",
                table: "DichVus",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_CCCD",
                table: "KhachHangs",
                column: "CCCD",
                unique: true);
        }
    }
}
