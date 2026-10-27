using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class LichPhongHelper
{
    // Khoảng lưu trú [nhận, trả): hai lượt nối tiếp nhau không trùng lịch.
    // Chỉ dùng phòng chính làm lịch dự phòng khi booking chưa có chi tiết.
    public static IQueryable<int> PhongBiTrung(AppDbContext db, DateTime nhan, DateTime tra,
        int? boQuaDatPhong = null, int? boQuaChiTiet = null, int? boQuaPhongChinh = null)
    {
        var phongChinh = db.DatPhongs.Where(d =>
            (!boQuaDatPhong.HasValue || d.MaDatPhong != boQuaDatPhong.Value)
            && (!boQuaPhongChinh.HasValue || d.MaDatPhong != boQuaPhongChinh.Value)
            && d.TrangThai != TrangThaiDatPhong.Huy
            && d.TrangThai != TrangThaiDatPhong.TuChoi
            && d.TrangThai != TrangThaiDatPhong.DaTraPhong
            && !d.ChiTietDatPhongs.Any()
            && d.NgayNhan < tra && d.NgayTraDuKien > nhan)
            .Select(d => d.MaPhong);

        var chiTiet = db.ChiTietDatPhongs.Where(c =>
            (!boQuaDatPhong.HasValue || c.MaDatPhong != boQuaDatPhong.Value)
            && (!boQuaChiTiet.HasValue || c.MaChiTiet != boQuaChiTiet.Value)
            && c.TrangThai != TrangThaiChiTiet.DaHuy
            && c.DatPhong.TrangThai != TrangThaiDatPhong.Huy
            && c.DatPhong.TrangThai != TrangThaiDatPhong.TuChoi
            && c.NgayNhan < tra && c.NgayTra > nhan)
            .Select(c => c.MaPhong);

        return phongChinh.Union(chiTiet);
    }

    public static IQueryable<int> PhongDangO(AppDbContext db, int? boQuaDatPhong = null) =>
        db.ChiTietDatPhongs.Where(c => c.TrangThai == TrangThaiChiTiet.DangO
            && (!boQuaDatPhong.HasValue || c.MaDatPhong != boQuaDatPhong.Value))
            .Select(c => c.MaPhong)
        .Union(db.DatPhongs.Where(d => d.TrangThai == TrangThaiDatPhong.DaNhanPhong
            && !d.ChiTietDatPhongs.Any()
            && (!boQuaDatPhong.HasValue || d.MaDatPhong != boQuaDatPhong.Value))
            .Select(d => d.MaPhong));

    public static IQueryable<Phong> PhongKinhDoanh(AppDbContext db) =>
        db.Phongs.Where(p => p.LoaiPhong.TrangThai
            && (p.TrangThai == TrangThaiPhong.TrongRanh || p.TrangThai == TrangThaiPhong.Trong
                || p.TrangThai == "SanSang" || p.TrangThai == TrangThaiPhong.DaDat
                || p.TrangThai == TrangThaiPhong.DangSuDung));

    public static Task<bool> CoTrungAsync(AppDbContext db, int maPhong, DateTime nhan, DateTime tra,
        int? boQuaDatPhong = null) =>
        PhongBiTrung(db, nhan, tra, boQuaDatPhong).ContainsAsync(maPhong);
}
