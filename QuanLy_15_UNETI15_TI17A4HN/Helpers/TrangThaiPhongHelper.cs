using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class TrangThaiPhongHelper
{
    public static string? ChuanHoa(string? trangThai) => trangThai?.Trim() switch
    {
        "Trong" or "TrongRanh" or "SanSang" or "Trống" or "Còn trống" => TrangThaiPhong.TrongRanh,
        "DaDat" or "Đã đặt" => TrangThaiPhong.DaDat,
        "DangSuDung" or "Đang sử dụng" => TrangThaiPhong.DangSuDung,
        "BaoTri" or "Bảo trì" => TrangThaiPhong.BaoTri,
        "KhongKinhDoanh" or "NgungKinhDoanh" or "Ngừng kinh doanh" => TrangThaiPhong.KhongKinhDoanh,
        _ => null
    };

    // Gọi trong giao dịch khóa lịch phòng, trước khi cập nhật trạng thái.
    public static async Task<string?> KiemTraAsync(AppDbContext db, int maPhong, string trangThai)
    {
        var dangO = await LichPhongHelper.PhongDangO(db).ContainsAsync(maPhong);
        if (dangO && trangThai != TrangThaiPhong.DangSuDung)
            return "Phòng vẫn có khách đang ở. Hãy hoàn tất trả phòng trước khi đổi trạng thái.";
        if (!dangO && trangThai == TrangThaiPhong.DangSuDung)
            return "Hãy sử dụng chức năng Nhận phòng để chuyển phòng sang đang sử dụng.";
        if (trangThai is TrangThaiPhong.BaoTri or TrangThaiPhong.KhongKinhDoanh)
        {
            var coLich = await db.ChiTietDatPhongs.AnyAsync(c => c.MaPhong == maPhong
                && c.TrangThai == TrangThaiChiTiet.ChuaNhan && c.NgayTra > DateTime.Today
                && (c.DatPhong.TrangThai == TrangThaiDatPhong.ChoDuyet || c.DatPhong.TrangThai == TrangThaiDatPhong.DaDuyet))
                || await db.DatPhongs.AnyAsync(d => d.MaPhong == maPhong && !d.ChiTietDatPhongs.Any()
                    && d.NgayTraDuKien > DateTime.Today
                    && (d.TrangThai == TrangThaiDatPhong.ChoDuyet || d.TrangThai == TrangThaiDatPhong.DaDuyet));
            if (coLich) return "Phòng còn lịch đặt chưa xử lý. Hãy đổi phòng hoặc hủy lịch trước khi ngừng kinh doanh/bảo trì.";
        }
        return null;
    }
}
