// Migration cũ từng xóa toàn bộ đặt phòng, chi tiết đặt phòng và khách hàng rời.
// Đã vô hiệu hóa phần xóa để không làm mất dữ liệu khi cập nhật cơ sở dữ liệu hiện có.
// Giữ nguyên migration nhằm bảo toàn lịch sử migration của các cơ sở dữ liệu đã chạy bản cũ.
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
            // Không xóa dữ liệu. Dữ liệu mẫu được nạp idempotent trong DuLieuMauGiaoDich.Seed.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không khôi phục dữ liệu mẫu đã xóa.
        }
    }
}
