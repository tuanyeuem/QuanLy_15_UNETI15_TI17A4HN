// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Chọn màu badge hiển thị theo trạng thái nghiệp vụ.
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class TrangThaiUi
{
    public static string Badge(string? trangThai) => trangThai switch
    {
        TrangThaiDatPhong.ChoXuLy or TrangThaiChiTiet.ChuaNhan => "bg-warning text-dark",
        TrangThaiDatPhong.DangXuLy or TrangThaiChiTiet.DangO => "bg-primary",
        TrangThaiDatPhong.HoanThanh or TrangThaiChiTiet.DaTra => "bg-success",
        TrangThaiDatPhong.DaHuy => "bg-secondary",
        _ => "bg-light text-dark"
    };

    public static string Tien(decimal soTien) => soTien.ToString("N0") + " đ";
}
