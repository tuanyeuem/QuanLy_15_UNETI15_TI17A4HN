using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLy_15_UNETI15_TI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class MergeBookingRoomLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.Sql("""
                UPDATE dp
                SET MaPhong = COALESCE(
                    (SELECT TOP (1) ct.MaPhong
                     FROM ChiTietDatPhongs AS ct
                     WHERE ct.MaDatPhong = dp.MaDatPhong
                     ORDER BY ct.MaChiTiet),
                    (SELECT MIN(p.MaPhong) FROM Phongs AS p))
                FROM DatPhongs AS dp
                WHERE dp.MaPhong = 0;
                """);

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

            // Earlier schema branches did not include this nullable audit field.
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'dbo.ChiTietDatPhongs', N'NgayCapNhat') IS NULL
                    ALTER TABLE [dbo].[ChiTietDatPhongs] ADD [NgayCapNhat] datetime2 NULL;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.check_constraints
                    WHERE name = N'CK_ChiTietDatPhong_GiaTri'
                      AND parent_object_id = OBJECT_ID(N'dbo.ChiTietDatPhongs'))
                BEGIN
                    ALTER TABLE [dbo].[ChiTietDatPhongs] WITH CHECK
                    ADD CONSTRAINT [CK_ChiTietDatPhong_GiaTri]
                    CHECK ([DonGia] >= 0 AND [SoNgay] >= 0 AND [ThanhTien] >= 0 AND [NgayTra] >= [NgayNhan]);
                END;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_DatPhongs_MaPhong",
                table: "DatPhongs",
                column: "MaPhong");

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

            migrationBuilder.DropIndex(
                name: "IX_DatPhongs_MaPhong",
                table: "DatPhongs");

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
        }
    }
}
