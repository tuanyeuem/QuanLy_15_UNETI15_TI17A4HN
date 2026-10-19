// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Dashboard, thống kê bằng LINQ (Count, Sum, GroupBy, OrderBy...),
// báo cáo doanh thu theo khoảng ngày và xuất CSV.
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

public class ThongKeController : Controller
{
    private readonly AppDbContext _context;

    public ThongKeController(AppDbContext context) => _context = context;

    // ===================== DASHBOARD =====================

    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> Dashboard()
    {
        // GroupBy(TrangThai) + Count(): gom các lần đặt phòng theo trạng thái và đếm mỗi nhóm bằng 1 câu SQL
        // (GROUP BY). Ý nghĩa: biết ngay bao nhiêu giao dịch đang chờ xử lý / đang xử lý / hoàn thành / đã hủy.
        var theoTrangThai = await _context.DatPhongs.AsNoTracking()
            .GroupBy(d => d.TrangThai)
            .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
            .ToListAsync();

        int Dem(string trangThai) => theoTrangThai.Where(x => x.TrangThai == trangThai).Sum(x => x.SoLuong);

        var vm = new DashboardViewModel
        {
            TongLoaiPhong = await _context.LoaiPhongs.CountAsync(),
            TongPhong = await _context.Phongs.CountAsync(),
            PhongKhaDung = await _context.Phongs.CountAsync(p => p.TrangThai == TrangThaiPhong.SanSang),
            TongKhachHang = await _context.KhachHangs.CountAsync(),
            TongGiaoDich = theoTrangThai.Sum(x => x.SoLuong),
            ChoXuLy = Dem(TrangThaiDatPhong.ChoXuLy),
            DangXuLy = Dem(TrangThaiDatPhong.DangXuLy),
            HoanThanh = Dem(TrangThaiDatPhong.HoanThanh),
            DaHuy = Dem(TrangThaiDatPhong.DaHuy),
            // Where(Đã trả) + Sum(ThanhTien): chỉ cộng tiền các phòng đã chốt kết quả; (decimal?) và ?? 0 để
            // trả về 0 thay vì lỗi khi chưa có dòng nào. Ý nghĩa: tổng doanh thu thực tế đã phát sinh.
            TongDoanhThu = await _context.ChiTietDatPhongs
                .Where(c => c.TrangThai == TrangThaiChiTiet.DaTra)
                .SumAsync(c => (decimal?)c.ThanhTien) ?? 0,
            GanDay = await _context.DatPhongs.AsNoTracking()
                .Include(d => d.KhachHang)
                .OrderByDescending(d => d.NgayDat)
                .Take(5)
                .ToListAsync()
        };
        return View(vm);
    }

    // ===================== THỐNG KÊ =====================

    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> Index(int? nam)
    {
        var namChon = nam ?? DateTime.Today.Year;

        var cacNam = await _context.DatPhongs.AsNoTracking()
            .Select(d => d.NgayDat.Year).Distinct()
            .OrderByDescending(y => y).ToListAsync();
        if (!cacNam.Contains(DateTime.Today.Year)) cacNam.Insert(0, DateTime.Today.Year);
        if (!cacNam.Contains(namChon)) cacNam.Add(namChon);

        var vm = new ThongKeViewModel { Nam = namChon, CacNam = cacNam.OrderByDescending(y => y).ToList() };

        // 1 + 2. Số phòng theo loại và tỷ lệ sử dụng: với mỗi loại phòng, Count() tổng số phòng và Count() số phòng
        // đang sử dụng; OrderBy(TenLoai) để hiển thị theo thứ tự chữ cái.
        // Ý nghĩa: loại phòng nào có tỷ lệ cao là đang được khai thác tốt (hoặc sắp hết phòng).
        vm.TyLeSuDung = await _context.LoaiPhongs.AsNoTracking()
            .OrderBy(l => l.TenLoai)
            .Select(l => new TyLeSuDungLoaiPhong
            {
                TenLoai = l.TenLoai,
                TongPhong = _context.Phongs.Count(p => p.MaLoaiPhong == l.MaLoaiPhong),
                DangSuDung = _context.Phongs.Count(p => p.MaLoaiPhong == l.MaLoaiPhong
                                                     && p.TrangThai == TrangThaiPhong.DangSuDung)
            }).ToListAsync();
        vm.SoPhongTheoLoai = vm.TyLeSuDung
            .Select(t => new SoLuongTheoTen { Ten = t.TenLoai, SoLuong = t.TongPhong }).ToList();

        vm.PhongDangSuDung = await _context.Phongs.CountAsync(p => p.TrangThai == TrangThaiPhong.DangSuDung);
        vm.PhongKinhDoanh = await _context.Phongs.CountAsync(p => p.TrangThai != TrangThaiPhong.BaoTri);

        // 3. Số lượt đặt theo tháng: Where(năm) -> GroupBy(tháng của NgayDat) -> Count().
        // Ý nghĩa: tháng nào đông khách (mùa cao điểm), tháng nào vắng.
        var luotDat = await _context.DatPhongs.AsNoTracking()
            .Where(d => d.NgayDat.Year == namChon)
            .GroupBy(d => d.NgayDat.Month)
            .Select(g => new { Thang = g.Key, SoLuot = g.Count() })
            .ToListAsync();

        // 4. Doanh thu theo tháng: Where(Đã trả, năm) -> GroupBy(tháng của NgayTra) -> Sum(ThanhTien).
        // Tính theo ngày trả thực tế vì lúc đó tiền mới được chốt. Ý nghĩa: biến động doanh thu theo thời gian.
        var doanhThu = await _context.ChiTietDatPhongs.AsNoTracking()
            .Where(c => c.TrangThai == TrangThaiChiTiet.DaTra && c.NgayTra.Year == namChon)
            .GroupBy(c => c.NgayTra.Month)
            .Select(g => new { Thang = g.Key, DoanhThu = g.Sum(c => c.ThanhTien) })
            .ToListAsync();

        vm.TheoThang = Enumerable.Range(1, 12).Select(t => new GiaTriTheoThang
        {
            Thang = t,
            SoLuot = luotDat.Where(x => x.Thang == t).Sum(x => x.SoLuot),
            DoanhThu = doanhThu.Where(x => x.Thang == t).Sum(x => x.DoanhThu)
        }).ToList();

        // 5. Doanh thu theo loại phòng: GroupBy(tên loại) -> Count() số lượt + Sum(ThanhTien) -> OrderByDescending(doanh thu).
        // Ý nghĩa: loại phòng nào mang lại nhiều tiền nhất (đứng đầu danh sách).
        vm.DoanhThuTheoLoai = await _context.ChiTietDatPhongs.AsNoTracking()
            .Where(c => c.TrangThai == TrangThaiChiTiet.DaTra && c.NgayTra.Year == namChon)
            .GroupBy(c => c.Phong.LoaiPhong.TenLoai)
            .Select(g => new DoanhThuTheoLoai
            {
                TenLoai = g.Key,
                SoLuot = g.Count(),
                DoanhThu = g.Sum(c => c.ThanhTien)
            })
            .OrderByDescending(x => x.DoanhThu)
            .ToListAsync();

        // 6. Phòng được đặt nhiều nhất: loại bỏ chi tiết đã hủy -> GroupBy(phòng) -> Count() -> OrderByDescending(số lượt)
        // -> ThenBy(số phòng) để kết quả ổn định khi hòa -> Take(5). Ý nghĩa: phòng được khách ưa chuộng nhất.
        vm.PhongDatNhieuNhat = await _context.ChiTietDatPhongs.AsNoTracking()
            .Where(c => c.TrangThai != TrangThaiChiTiet.DaHuy)
            .GroupBy(c => new { c.MaPhong, c.Phong.SoPhong, c.Phong.LoaiPhong.TenLoai })
            .Select(g => new PhongDatNhieu
            {
                SoPhong = g.Key.SoPhong,
                TenLoai = g.Key.TenLoai,
                SoLuot = g.Count(),
                TongTien = g.Sum(c => c.ThanhTien)
            })
            .OrderByDescending(x => x.SoLuot).ThenBy(x => x.SoPhong)
            .Take(5)
            .ToListAsync();

        // 7. Số lượt hủy: Count() có điều kiện (trạng thái Đã hủy và đúng năm). Ý nghĩa: so với tổng lượt đặt để
        // thấy tỷ lệ khách hủy; tỷ lệ cao cần xem lại quy trình đặt/xác nhận.
        // Any(): kiểm tra nhanh năm đang chọn có dữ liệu hay không (SQL EXISTS, nhanh hơn Count() > 0)
        // để giao diện hiển thị thông báo thay vì bảng toàn số 0.
        vm.CoDuLieu = await _context.DatPhongs.AnyAsync(d => d.NgayDat.Year == namChon)
                   || await _context.ChiTietDatPhongs.AnyAsync(c => c.TrangThai == TrangThaiChiTiet.DaTra
                                                                 && c.NgayTra.Year == namChon);
        vm.TongLuotDatTrongNam = luotDat.Sum(x => x.SoLuot);
        vm.SoLuotHuyTrongNam = await _context.DatPhongs
            .CountAsync(d => d.TrangThai == TrangThaiDatPhong.DaHuy && d.NgayDat.Year == namChon);
        vm.TongDoanhThuTrongNam = vm.TheoThang.Sum(t => t.DoanhThu);

        return View(vm);
    }

    // ===================== BÁO CÁO =====================

    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> BaoCao(DateTime? tuNgay, DateTime? denNgay)
    {
        var (tu, den) = ChuanHoaKhoangNgay(tuNgay, denNgay);
        if (den < tu)
        {
            ModelState.AddModelError(string.Empty, "Đến ngày phải lớn hơn hoặc bằng từ ngày.");
            (tu, den) = ChuanHoaKhoangNgay(null, null);
        }
        return View(await TaoBaoCaoAsync(tu, den));
    }

    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> XuatCsv(DateTime? tuNgay, DateTime? denNgay)
    {
        var (tu, den) = ChuanHoaKhoangNgay(tuNgay, denNgay);
        if (den < tu) (tu, den) = ChuanHoaKhoangNgay(null, null);
        var bc = await TaoBaoCaoAsync(tu, den);

        static string Cot(string? s) => "\"" + (s ?? string.Empty).Replace("\"", "\"\"") + "\"";

        var sb = new StringBuilder();
        sb.AppendLine("Mã chi tiết,Mã đặt phòng,Khách hàng,Số phòng,Loại phòng,Ngày nhận,Ngày trả,Đơn giá,Số ngày,Thành tiền");
        foreach (var d in bc.Dong)
        {
            sb.AppendLine(string.Join(",",
                d.MaChiTiet, d.MaDatPhong, Cot(d.DatPhong.KhachHang.HoTen), Cot(d.Phong.SoPhong),
                Cot(d.Phong.LoaiPhong.TenLoai), d.NgayNhan.ToString("dd/MM/yyyy"), d.NgayTra.ToString("dd/MM/yyyy"),
                d.DonGia.ToString("0"), d.SoNgay, d.ThanhTien.ToString("0")));
        }
        sb.AppendLine($"Tổng doanh thu,,,,,,,,{bc.TongSoDem},{bc.TongDoanhThu:0}");

        // Thêm BOM UTF-8 để Excel hiển thị đúng tiếng Việt.
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv", $"BaoCaoDoanhThu_{tu:yyyyMMdd}_{den:yyyyMMdd}.csv");
    }

    private static (DateTime Tu, DateTime Den) ChuanHoaKhoangNgay(DateTime? tuNgay, DateTime? denNgay)
    {
        var den = (denNgay ?? DateTime.Today).Date;
        var tu = (tuNgay ?? new DateTime(den.Year, 1, 1)).Date;
        return (tu, den);
    }

    private async Task<BaoCaoViewModel> TaoBaoCaoAsync(DateTime tu, DateTime den)
    {
        var denTruocNgaySau = den.AddDays(1);

        var dong = await _context.ChiTietDatPhongs.AsNoTracking()
            .Include(c => c.Phong).ThenInclude(p => p.LoaiPhong)
            .Include(c => c.DatPhong).ThenInclude(d => d.KhachHang)
            .Where(c => c.TrangThai == TrangThaiChiTiet.DaTra
                     && c.NgayTra >= tu && c.NgayTra < denTruocNgaySau)
            .OrderBy(c => c.NgayTra).ThenBy(c => c.MaChiTiet)
            .ToListAsync();

        return new BaoCaoViewModel
        {
            TuNgay = tu,
            DenNgay = den,
            Dong = dong,
            TheoLoai = dong.GroupBy(c => c.Phong.LoaiPhong.TenLoai)
                .Select(g => new DoanhThuTheoLoai
                {
                    TenLoai = g.Key,
                    SoLuot = g.Count(),
                    DoanhThu = g.Sum(c => c.ThanhTien)
                })
                .OrderByDescending(x => x.DoanhThu)
                .ToList()
        };
    }
}
