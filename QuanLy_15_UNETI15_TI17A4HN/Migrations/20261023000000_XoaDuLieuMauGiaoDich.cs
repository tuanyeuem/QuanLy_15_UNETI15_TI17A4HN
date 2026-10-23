// Dọn dữ liệu mẫu: xóa toàn bộ đặt phòng, chi tiết đặt phòng, sử dụng dịch vụ
// và các khách hàng không gắn với tài khoản. Giữ lại tài khoản, khách hàng liên kết tài khoản, phòng, loại phòng, dịch vụ.
// Migration này chỉ chạy một lần (db.Database.Migrate() trong Program.cs).
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using QuanLy_15_UNETI15_TI17A4HN.Data;

#nullable disable

namespace QuanLy_15_UNETI15_TI17A4HN.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261023000000_XoaDuLieuMauGiaoDich")]
    public partial class XoaDuLieuMauGiaoDich : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [SuDungDichVus];");
            migrationBuilder.Sql("DELETE FROM [ChiTietDatPhongs];");
            migrationBuilder.Sql("DELETE FROM [DatPhongs];");
            migrationBuilder.Sql("DELETE FROM [KhachHangs] WHERE [MaTaiKhoan] IS NULL;");

            // Phòng đang "đang sử dụng" vì đặt phòng mẫu -> trả về trống/sẵn sàng.
            migrationBuilder.Sql("UPDATE [Phongs] SET [TrangThai] = N'TrongRanh' WHERE [TrangThai] = N'DangSuDung';");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không khôi phục dữ liệu mẫu đã xóa.
        }
    }
}
