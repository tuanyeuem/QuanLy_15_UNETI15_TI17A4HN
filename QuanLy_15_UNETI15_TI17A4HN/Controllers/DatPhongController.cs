// Họ và tên: Vũ Xuân Tuấn
// Mã sinh viên: 23103100035
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
        // Tránh chia cho 0 nếu người dùng sửa query string.
        vm.SoTrangHienThi = vm.SoTrangHienThi is < 1 or > 100 ? 10 : vm.SoTrangHienThi;

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
        await using var giaoDich = await GiaoDichLichPhong.BatDauAsync(_db);
        var dp = await _db.DatPhongs
            .Include(d => d.Phong).ThenInclude(p => p.LoaiPhong)
            .Include(d => d.ChiTietDatPhongs).ThenInclude(c => c.Phong).ThenInclude(p => p.LoaiPhong)
            .FirstOrDefaultAsync(d => d.MaDatPhong == id);

        if (dp == null) return NotFound();

        if (dp.TrangThai != TrangThaiDatPhong.ChoDuyet)
        { TempData["Loi"] = "Trạng thái không hợp lệ."; return RedirectToAction(nameof(Index)); }

        // ── Kiểm tra nghiệp vụ ──
        if (dp.NgayTraDuKien <= dp.NgayNhan)
        { TempData["Loi"] = "Ngày trả phải sau ngày nhận."; return RedirectToAction(nameof(ChiTiet), new { id }); }

        var chiTietHoatDong = dp.ChiTietDatPhongs
            .Where(c => c.TrangThai != TrangThaiChiTiet.DaHuy)
            .ToList();

        if (!dp.ChiTietDatPhongs.Any())
        {
            // Đặt phòng cũ có thể chưa có ChiTietDatPhong: kiểm tra theo phòng chính.
            if (dp.Phong.LoaiPhong == null)
            {
                TempData["Loi"] = "Phòng chính không còn loại phòng hợp lệ.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            if (dp.SoNguoi > dp.Phong.LoaiPhong.SoNguoiToiDa)
            {
                TempData["Loi"] = $"Số người ({dp.SoNguoi}) vượt quá sức chứa ({dp.Phong.LoaiPhong.SoNguoiToiDa}) của phòng chính.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            if (dp.Phong.TrangThai is not (TrangThaiPhong.TrongRanh or TrangThaiPhong.DangSuDung))
            {
                TempData["Loi"] = "Phòng không ở trạng thái có thể đặt.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            if (await CoLichTrungAsync(dp.Phong.MaPhong, dp.NgayNhan, dp.NgayTraDuKien, dp.MaDatPhong))
            {
                TempData["Loi"] = "Phòng đã có lịch đặt trùng thời gian.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }
        }
        else
        {
            if (chiTietHoatDong.GroupBy(c => c.MaPhong).Any(g => g.Count() > 1))
            {
                TempData["Loi"] = "Đặt phòng có phòng bị lặp trong danh sách chi tiết.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            var tongSucChua = 0;
            foreach (var chiTiet in chiTietHoatDong)
            {
                if (chiTiet.Phong?.LoaiPhong == null)
                {
                    TempData["Loi"] = "Có phòng trong chi tiết không còn tồn tại.";
                    return RedirectToAction(nameof(ChiTiet), new { id });
                }

                if (chiTiet.NgayTra <= chiTiet.NgayNhan)
                {
                    TempData["Loi"] = $"Phòng {chiTiet.Phong.SoPhong}: ngày trả phải sau ngày nhận.";
                    return RedirectToAction(nameof(ChiTiet), new { id });
                }

                if (chiTiet.NgayNhan.Date < dp.NgayNhan.Date
                    || chiTiet.NgayTra.Date > dp.NgayTraDuKien.Date)
                {
                    TempData["Loi"] = $"Phòng {chiTiet.Phong.SoPhong}: thời gian nằm ngoài khoảng đặt phòng.";
                    return RedirectToAction(nameof(ChiTiet), new { id });
                }

                if (chiTiet.Phong.TrangThai is not (TrangThaiPhong.TrongRanh or TrangThaiPhong.DangSuDung))
                {
                    TempData["Loi"] = $"Phòng {chiTiet.Phong.SoPhong} không ở trạng thái có thể đặt.";
                    return RedirectToAction(nameof(ChiTiet), new { id });
                }

                if (await CoLichTrungAsync(chiTiet.MaPhong, chiTiet.NgayNhan, chiTiet.NgayTra, dp.MaDatPhong))
                {
                    TempData["Loi"] = $"Phòng {chiTiet.Phong.SoPhong} đã có lịch đặt trùng thời gian.";
                    return RedirectToAction(nameof(ChiTiet), new { id });
                }

                tongSucChua += chiTiet.Phong.LoaiPhong.SoNguoiToiDa;
            }

            if (dp.SoNguoi > tongSucChua)
            {
                TempData["Loi"] = $"Số người ({dp.SoNguoi}) vượt quá tổng sức chứa ({tongSucChua}) của các phòng đã chọn.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }
        }

        dp.TrangThai = TrangThaiDatPhong.DaDuyet;
        if (!string.IsNullOrWhiteSpace(ghiChu)) dp.GhiChu = ghiChu;

        await _db.SaveChangesAsync();
        await giaoDich.CommitAsync();
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
        await using var giaoDich = await GiaoDichLichPhong.BatDauAsync(_db);
        var dp = await _db.DatPhongs
            .Include(d => d.Phong).ThenInclude(p => p.LoaiPhong)
            .Include(d => d.ChiTietDatPhongs).ThenInclude(c => c.Phong).ThenInclude(p => p.LoaiPhong)
            .FirstOrDefaultAsync(d => d.MaDatPhong == id);
        if (dp == null) return NotFound();

        if (dp.TrangThai != TrangThaiDatPhong.DaDuyet)
        { TempData["Loi"] = "Trạng thái không hợp lệ."; return RedirectToAction(nameof(Index)); }

        var thoiDiemNhan = DateTime.Now;
        if (thoiDiemNhan.Date > dp.NgayTraDuKien.Date)
        {
            TempData["Loi"] = "Không thể nhận phòng sau ngày trả dự kiến. Hãy cập nhật lại thời gian đặt trước khi nhận phòng.";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        // Bảo đảm đặt phòng cũ cũng có chi tiết để Module 5 tính tiền được.
        if (!dp.ChiTietDatPhongs.Any())
        {
            var soNgayDuKien = TinhTienHelper.TinhSoNgay(thoiDiemNhan.Date, dp.NgayTraDuKien.Date);
            dp.ChiTietDatPhongs.Add(new ChiTietDatPhong
            {
                MaPhong = dp.MaPhong,
                Phong = dp.Phong,
                NgayNhan = thoiDiemNhan.Date,
                NgayTra = dp.NgayTraDuKien.Date,
                DonGia = dp.Phong.DonGia,
                SoNgay = soNgayDuKien,
                ThanhTien = TinhTienHelper.TinhThanhTien(dp.Phong.DonGia, soNgayDuKien),
                TrangThai = TrangThaiChiTiet.ChuaNhan
            });
        }

        var chiTietCanNhan = dp.ChiTietDatPhongs
            .Where(c => c.TrangThai == TrangThaiChiTiet.ChuaNhan || c.TrangThai == TrangThaiChiTiet.DangO)
            .ToList();

        if (chiTietCanNhan.Count == 0)
        {
            TempData["Loi"] = "Đặt phòng không còn chi tiết nào có thể nhận.";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        if (chiTietCanNhan.GroupBy(c => c.MaPhong).Any(g => g.Count() > 1))
        {
            TempData["Loi"] = "Đặt phòng có phòng bị lặp trong danh sách chi tiết.";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        foreach (var chiTiet in chiTietCanNhan)
        {
            if (chiTiet.Phong == null)
            {
                TempData["Loi"] = "Có phòng trong chi tiết không còn tồn tại.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            if (chiTiet.Phong.TrangThai is TrangThaiPhong.BaoTri
                or TrangThaiPhong.KhongKinhDoanh
                or TrangThaiPhong.NgungKinhDoanh)
            {
                TempData["Loi"] = $"Phòng {chiTiet.Phong.SoPhong} đang bảo trì/ngừng kinh doanh, không thể nhận phòng.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            // Giữ nguyên ngày riêng của từng phòng nếu hợp lệ; chỉ sửa khi dữ liệu cũ bị thiếu.
            var ngayNhan = chiTiet.NgayNhan.Date;
            var ngayTra = chiTiet.NgayTra.Date;

            if (ngayNhan < dp.NgayNhan.Date)
                ngayNhan = dp.NgayNhan.Date;
            if (ngayTra <= ngayNhan || ngayTra > dp.NgayTraDuKien.Date)
                ngayTra = dp.NgayTraDuKien.Date;

            if (ngayTra <= ngayNhan)
            {
                TempData["Loi"] = $"Phòng {chiTiet.Phong.SoPhong}: ngày trả không hợp lệ.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            if (await CoLichTrungAsync(chiTiet.MaPhong, ngayNhan, ngayTra, dp.MaDatPhong))
            {
                TempData["Loi"] = $"Phòng {chiTiet.Phong.SoPhong} đã có lịch đặt trùng thời gian.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            var donGia = chiTiet.DonGia > 0 ? chiTiet.DonGia : chiTiet.Phong.DonGia;

            chiTiet.NgayNhan = ngayNhan;
            chiTiet.NgayTra = ngayTra;
            chiTiet.DonGia = donGia;
            chiTiet.SoNgay = TinhTienHelper.TinhSoNgay(ngayNhan, ngayTra);
            chiTiet.ThanhTien = TinhTienHelper.TinhThanhTien(donGia, chiTiet.SoNgay);
            chiTiet.TrangThai = TrangThaiChiTiet.DangO;
            chiTiet.NgayCapNhat = thoiDiemNhan;
            chiTiet.Phong.TrangThai = TrangThaiPhong.DangSuDung;
        }

        dp.TrangThai = TrangThaiDatPhong.DaNhanPhong;
        dp.NgayNhanThucTe = thoiDiemNhan;

        await _db.SaveChangesAsync();
        await giaoDich.CommitAsync();
        TempData["ThanhCong"] = "Đã xác nhận khách nhận phòng.";
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    // ══════════════════════════════════════════
    //  TRẢ PHÒNG: DaNhanPhong → Module 5 chốt tiền và hoàn thành
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

        // Module 5 tính tiền, cập nhật chi tiết lưu trú và hoàn tất đặt phòng.
        return RedirectToAction("ChotKetQua", "ChiTietDatPhong", new
        {
            maDatPhong = id,
            ngayTra = DateTime.Today
        });
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
        await using var giaoDich = await GiaoDichLichPhong.BatDauAsync(_db);
        var dp = await _db.DatPhongs.Include(d => d.ChiTietDatPhongs)
            .FirstOrDefaultAsync(d => d.MaDatPhong == id);
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
        foreach (var chiTiet in dp.ChiTietDatPhongs.Where(c => c.TrangThai == TrangThaiChiTiet.ChuaNhan))
        {
            chiTiet.TrangThai = TrangThaiChiTiet.DaHuy;
            chiTiet.NgayCapNhat = DateTime.Now;
        }

        await _db.SaveChangesAsync();
        await giaoDich.CommitAsync();
        TempData["ThanhCong"] = "Đã hủy đặt phòng.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helper ──
    private async Task<DatPhong?> LayDatPhong(int id)
        => await _db.DatPhongs
            .Include(d => d.KhachHang)
            .Include(d => d.Phong).ThenInclude(p => p.LoaiPhong)
            .Include(d => d.ChiTietDatPhongs).ThenInclude(c => c.Phong).ThenInclude(p => p.LoaiPhong)
            .FirstOrDefaultAsync(d => d.MaDatPhong == id);

    /// <summary>
    /// Kiểm tra một phòng đã bị đặt trùng khoảng ngày hay chưa.
    /// Bỏ qua chính đặt phòng đang xét và các đặt phòng đã hủy/từ chối.
    /// </summary>
    private async Task<bool> CoLichTrungAsync(int maPhong, DateTime ngayNhan, DateTime ngayTra, int maDatPhongBoQua)
    {
        var trungDatPhong = await _db.DatPhongs.AnyAsync(d =>
            d.MaPhong == maPhong
            && d.MaDatPhong != maDatPhongBoQua
            && d.TrangThai != TrangThaiDatPhong.Huy
            && d.TrangThai != TrangThaiDatPhong.TuChoi
            && d.TrangThai != TrangThaiDatPhong.HoanThanh
            && !d.ChiTietDatPhongs.Any()
            && d.NgayNhan < ngayTra
            && d.NgayTraDuKien > ngayNhan);

        if (trungDatPhong) return true;

        return await _db.ChiTietDatPhongs.AnyAsync(c =>
            c.MaPhong == maPhong
            && c.MaDatPhong != maDatPhongBoQua
            && c.TrangThai != TrangThaiChiTiet.DaHuy
            && c.NgayNhan < ngayTra
            && c.NgayTra > ngayNhan);
    }
}
