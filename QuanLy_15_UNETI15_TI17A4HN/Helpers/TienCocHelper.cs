// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Module 3 - tính mức tiền cọc tối thiểu 20% và kiểm tra tiền cọc khi tạo đặt phòng (dùng lại ở Module 4).
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class TienCocHelper
{
    public static decimal ToiThieu(decimal tongTien) => Math.Ceiling(tongTien * 0.20m * 100m) / 100m;

    public static decimal TongTienPhong(DatPhong dp) => dp.ChiTietDatPhongs.Any()
        ? dp.ChiTietDatPhongs.Where(c => c.TrangThai != TrangThaiChiTiet.DaHuy).Sum(c => c.ThanhTien)
        : TinhTienHelper.TinhThanhTien(dp.Phong.DonGia, TinhTienHelper.TinhSoNgay(dp.NgayNhan, dp.NgayTraDuKien));

    public static string? KiemTra(decimal tienCoc, decimal tongTien) =>
        tienCoc < ToiThieu(tongTien) || tienCoc > tongTien || decimal.Round(tienCoc, 2) != tienCoc
            ? $"Tiền cọc phải từ {ToiThieu(tongTien):N2} đ (20%) đến {tongTien:N2} đ, tối đa 2 chữ số thập phân."
            : null;

    public static bool DuCoc(DatPhong dp) =>
        TongTienPhong(dp) == 0 || (dp.TienCocDaXacNhan && dp.TienCoc >= ToiThieu(TongTienPhong(dp)));
}
