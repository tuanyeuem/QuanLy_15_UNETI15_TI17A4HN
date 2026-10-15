// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 22103100001
// Nội dung thực hiện: DatPhongController - Quản lý duyệt, nhận phòng, trả phòng và hủy đặt phòng (Module 4)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

[PhanQuyen(VaiTroTaiKhoan.NhanVien, VaiTroTaiKhoan.Admin)]
public class DatPhongController : Controller
{
    private readonly AppDbContext _db;

    public DatPhongController(AppDbContext db) => _db = db;

    // ══════════════════════════════════════════
    //  INDEX — Danh sách + Tìm/Lọc/Sắp/Phân trang
    // ══════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> Index(DatPhongListViewModel vm)
    {
        var q = _db.DatPhongs
            .Include(d => d.KhachHang)
            .Include(d => d.Phong).ThenInclude(p => p.LoaiPhong)
            .AsQueryable();

        // Lấy danh sách Loại phòng (dữ liệu nền) cho dropdown bộ lọc
        vm.DanhSachLoaiPhong = await _db.LoaiPhongs.Where(l => l.TrangThai).ToListAsync();

        // Tìm kiếm theo khách hàng hoặc phòng
        if (!string.IsNullOrWhiteSpace(vm.TimKiem))
        {
            var kw = vm.TimKiem.Trim().ToLower();
            q = q.Where(d => d.KhachHang.HoTen.ToLower().Contains(kw)
                           || d.Phong.SoPhong.ToLower().Contains(kw));
        }

        // Lọc theo dữ liệu nền (Loại phòng)
        if (vm.MaLoaiPhong.HasValue && vm.MaLoaiPhong.Value > 0)
            q = q.Where(d => d.Phong.MaLoaiPhong == vm.MaLoaiPhong.Value);

        // Lọc theo trạng thái
        if (!string.IsNullOrWhiteSpace(vm.LocTrangThai))
            q = q.Where(d => d.TrangThai == vm.LocTrangThai);

        // Lọc theo khoảng ngày nhận
        if (vm.TuNgay.HasValue) q = q.Where(d => d.NgayNhan >= vm.TuNgay.Value);
        if (vm.DenNgay.HasValue) q = q.Where(d => d.NgayNhan <= vm.DenNgay.Value);

        // Sắp xếp tăng/giảm theo ngày hoặc theo tên
        q = vm.SapXep switch
        {
            "ngaydat_cu"      => q.OrderBy(d => d.NgayDat),
            "ngaynhan_moi"    => q.OrderByDescending(d => d.NgayNhan),
            "ngaynhan_cu"     => q.OrderBy(d => d.NgayNhan),
            "tenkhach_asc"    => q.OrderBy(d => d.KhachHang.HoTen),
            "tenkhach_desc"   => q.OrderByDescending(d => d.KhachHang.HoTen),
            _                 => q.OrderByDescending(d => d.NgayDat),   // mặc định: ngày đặt mới nhất
        };

        // Phân trang
        int tong = await q.CountAsync();
        vm.TongSoTrang  = (int)Math.Ceiling(tong / (double)vm.SoTrangHienThi);
        vm.TrangHienTai = Math.Max(1, Math.Min(vm.TrangHienTai, Math.Max(1, vm.TongSoTrang)));

        vm.DanhSach = await q
            .Skip((vm.TrangHienTai - 1) * vm.SoTrangHienThi)
            .Take(vm.SoTrangHienThi)
            .ToListAsync();

        return View(vm);
    }

    // ══════════════════════════════════════════
    //  CHI TIẾT
    // ══════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id)
    {
        var dp = await LayDatPhong(id);
        if (dp == null) return NotFound();
        return View(dp);
    }

    // ══════════════════════════════════════════
    //  DUYỆT: ChoDuyet → DaDuyet
    // ══════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> Duyet(int id)
    {
        var dp = await LayDatPhong(id);
        if (dp == null) return NotFound();
        if (dp.TrangThai != TrangThaiDatPhong.ChoDuyet)
        {
            TempData["Loi"] = "Chỉ duyệt được đặt phòng ở trạng thái Chờ duyệt.";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }
        return View(dp);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Duyet(int id, string? ghiChu)
    {
        var dp = await _db.DatPhongs
            .Include(d => d.Phong).ThenInclude(p => p.LoaiPhong)
            .FirstOrDefaultAsync(d => d.MaDatPhong == id);

        if (dp == null) return NotFound();

        if (dp.TrangThai != TrangThaiDatPhong.ChoDuyet)
        { TempData["Loi"] = "Trạng thái không hợp lệ."; return RedirectToAction(nameof(Index)); }

        // ── Kiểm tra nghiệp vụ ──
        if (dp.NgayTraDuKien <= dp.NgayNhan)
        { TempData["Loi"] = "Ngày trả phải sau ngày nhận."; return RedirectToAction(nameof(ChiTiet), new { id }); }

        if (dp.SoNguoi > dp.Phong.LoaiPhong.SoNguoiToiDa)
        { TempData["Loi"] = $"Số người ({dp.SoNguoi}) vượt quá sức chứa ({dp.Phong.LoaiPhong.SoNguoiToiDa})."; return RedirectToAction(nameof(ChiTiet), new { id }); }

        bool trungLich = await _db.DatPhongs.AnyAsync(d =>
            d.MaPhong == dp.MaPhong &&
            d.MaDatPhong != dp.MaDatPhong &&
            d.TrangThai != TrangThaiDatPhong.Huy &&
            d.NgayNhan < dp.NgayTraDuKien &&
            d.NgayTraDuKien > dp.NgayNhan);

        if (trungLich)
        { TempData["Loi"] = "Phòng đã có lịch đặt trùng thời gian."; return RedirectToAction(nameof(ChiTiet), new { id }); }

        if (dp.Phong.TrangThai is not ("TrongRanh" or "DangSuDung"))
        { TempData["Loi"] = "Phòng không ở trạng thái có thể đặt."; return RedirectToAction(nameof(ChiTiet), new { id }); }

        dp.TrangThai = TrangThaiDatPhong.DaDuyet;
        if (!string.IsNullOrWhiteSpace(ghiChu)) dp.GhiChu = ghiChu;

        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã duyệt đặt phòng.";
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    // ══════════════════════════════════════════
    //  NHẬN PHÒNG: DaDuyet → DaNhanPhong
    // ══════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> NhanPhong(int id)
    {
        var dp = await LayDatPhong(id);
        if (dp == null) return NotFound();
        if (dp.TrangThai != TrangThaiDatPhong.DaDuyet)
        { TempData["Loi"] = "Chỉ nhận phòng khi đã duyệt."; return RedirectToAction(nameof(ChiTiet), new { id }); }
        return View(dp);
    }

    // Đã đổi tên hàm C# thành NhanPhongPost và dùng ActionName("NhanPhong") để không vướng lỗi CS0111
    [HttpPost, ValidateAntiForgeryToken]
    [ActionName("NhanPhong")]
    public async Task<IActionResult> NhanPhongPost(int id)
    {
        var dp = await _db.DatPhongs.FindAsync(id);
        if (dp == null) return NotFound();

        if (dp.TrangThai != TrangThaiDatPhong.DaDuyet)
        { TempData["Loi"] = "Trạng thái không hợp lệ."; return RedirectToAction(nameof(Index)); }

        dp.TrangThai      = TrangThaiDatPhong.DaNhanPhong;
        dp.NgayNhanThucTe = DateTime.Now;   // Hệ thống xác định, không cho user nhập

        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã xác nhận khách nhận phòng.";
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    // ══════════════════════════════════════════
    //  TRẢ PHÒNG: DaNhanPhong → DaTraPhong
    // ══════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> TraPhong(int id)
    {
        var dp = await LayDatPhong(id);
        if (dp == null) return NotFound();
        if (dp.TrangThai != TrangThaiDatPhong.DaNhanPhong)
        { TempData["Loi"] = "Chỉ trả phòng khi khách đang ở."; return RedirectToAction(nameof(ChiTiet), new { id }); }
        return View(dp);
    }

    
    [HttpPost, ValidateAntiForgeryToken]
    [ActionName("TraPhong")]
    public async Task<IActionResult> TraPhongPost(int id)
    {
        var dp = await _db.DatPhongs.FindAsync(id);
        if (dp == null) return NotFound();

        if (dp.TrangThai != TrangThaiDatPhong.DaNhanPhong)
        { TempData["Loi"] = "Trạng thái không hợp lệ."; return RedirectToAction(nameof(Index)); }

        dp.TrangThai     = TrangThaiDatPhong.DaTraPhong;
        dp.NgayTraThucTe = DateTime.Now;    // Hệ thống xác định

        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = "Khách đã trả phòng thành công.";
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    // ══════════════════════════════════════════
    //  HỦY: ChoDuyet | DaDuyet → Huy
    // ══════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> Huy(int id)
    {
        var dp = await LayDatPhong(id);
        if (dp == null) return NotFound();
        if (dp.TrangThai is not (TrangThaiDatPhong.ChoDuyet or TrangThaiDatPhong.DaDuyet))
        { TempData["Loi"] = "Không thể hủy ở trạng thái này."; return RedirectToAction(nameof(ChiTiet), new { id }); }
        return View(dp);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int id, string lyDoHuy)
    {
        var dp = await _db.DatPhongs.FindAsync(id);
        if (dp == null) return NotFound();

        if (dp.TrangThai is not (TrangThaiDatPhong.ChoDuyet or TrangThaiDatPhong.DaDuyet))
        { TempData["Loi"] = "Không thể hủy."; return RedirectToAction(nameof(Index)); }

        if (string.IsNullOrWhiteSpace(lyDoHuy))
        {
            ModelState.AddModelError("", "Vui lòng nhập lý do hủy.");
            return View(await LayDatPhong(id));
        }

        dp.TrangThai = TrangThaiDatPhong.Huy;
        dp.LyDoHuy   = lyDoHuy;

        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã hủy đặt phòng.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helper ──
    private async Task<DatPhong?> LayDatPhong(int id)
        => await _db.DatPhongs
            .Include(d => d.KhachHang)
            .Include(d => d.Phong).ThenInclude(p => p.LoaiPhong)
            .FirstOrDefaultAsync(d => d.MaDatPhong == id);
}