// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Lớp phân trang dùng Skip()/Take() trên truy vấn EF Core.
using Microsoft.EntityFrameworkCore;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public interface IPhanTrang
{
    int TrangHienTai { get; }
    int TongSoTrang { get; }
    int TongSoDong { get; }
    bool CoTrangTruoc { get; }
    bool CoTrangSau { get; }
}

public class DanhSachPhanTrang<T> : IPhanTrang
{
    public List<T> Items { get; }
    public int TrangHienTai { get; }
    public int KichThuocTrang { get; }
    public int TongSoDong { get; }
    public int TongSoTrang => (int)Math.Ceiling(TongSoDong / (double)KichThuocTrang);
    public bool CoTrangTruoc => TrangHienTai > 1;
    public bool CoTrangSau => TrangHienTai < TongSoTrang;

    private DanhSachPhanTrang(List<T> items, int tongSoDong, int trang, int kichThuoc)
    {
        Items = items;
        TongSoDong = tongSoDong;
        TrangHienTai = trang;
        KichThuocTrang = kichThuoc;
    }

    // Phân trang ngay trên truy vấn: chỉ tải đúng một trang dữ liệu từ SQL Server.
    public static async Task<DanhSachPhanTrang<T>> TaoAsync(IQueryable<T> nguon, int trang, int kichThuoc)
    {
        if (kichThuoc < 1) kichThuoc = 10;
        var tong = await nguon.CountAsync();
        var tongTrang = Math.Max(1, (int)Math.Ceiling(tong / (double)kichThuoc));
        trang = Math.Clamp(trang, 1, tongTrang);

        var items = await nguon.Skip((trang - 1) * kichThuoc).Take(kichThuoc).ToListAsync();
        return new DanhSachPhanTrang<T>(items, tong, trang, kichThuoc);
    }
}
