// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Chi tiết đặt phòng: danh sách (tìm kiếm, lọc, sắp xếp, phân trang),
// thêm/sửa/xóa chi tiết, tính tiền phòng, chốt kết quả cuối cùng và xem kết quả.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

public class ChiTietDatPhongController : Controller
{
    private const int KichThuocTrang = 10;
    private readonly AppDbContext _context;

    public ChiTietDatPhongController(AppDbContext context) => _context = context;

    // ===================== DANH SÁCH (Admin / Nhân viên) =====================

    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> Index(string? tuKhoa, string? trangThai, int? maLoaiPhong,
        DateTime? tuNgay, DateTime? denNgay, string? sapXep, int trang = 1)
    {
        IQueryable<ChiTietDatPhong> query = _context.ChiTietDatPhongs.AsNoTracking()
            .Include(c => c.Phong).ThenInclude(p => p.LoaiPhong)
            .Include(c => c.DatPhong).ThenInclude(d => d.KhachHang);

        // Tìm kiếm: tên khách hàng, số phòng, tên loại phòng.
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var tk = tuKhoa.Trim();
            query = query.Where(c => c.DatPhong.KhachHang.HoTen.Contains(tk)
                                  || c.Phong.SoPhong.Contains(tk)
                                  || c.Phong.LoaiPhong.TenLoai.Contains(tk));
        }

        // Lọc (các điều kiện kết hợp được với nhau).
        if (!string.IsNullOrEmpty(trangThai))
            query = query.Where(c => c.TrangThai == trangThai);
        if (maLoaiPhong.HasValue)
            query = query.Where(c => c.Phong.MaLoaiPhong == maLoaiPhong.Value);
        if (tuNgay.HasValue)
            query = query.Where(c => c.NgayNhan >= tuNgay.Value.Date);
        if (denNgay.HasValue)
        {
            var den = denNgay.Value.Date.AddDays(1);
            query = query.Where(c => c.NgayNhan < den);
        }

        // Sắp xếp.
        sapXep ??= "ngay_giam";
        query = sapXep switch
        {
            "ngay_tang" => query.OrderBy(c => c.NgayNhan).ThenBy(c => c.MaChiTiet),
            "ten_az" => query.OrderBy(c => c.DatPhong.KhachHang.HoTen).ThenBy(c => c.MaChiTiet),
            "tien_giam" => query.OrderByDescending(c => c.ThanhTien).ThenBy(c => c.MaChiTiet),
            "tien_tang" => query.OrderBy(c => c.ThanhTien).ThenBy(c => c.MaChiTiet),
            _ => query.OrderByDescending(c => c.NgayNhan).ThenBy(c => c.MaChiTiet)
        };

        var vm = new ChiTietDatPhongIndexViewModel
        {
            TuKhoa = tuKhoa,
            TrangThai = trangThai,
            MaLoaiPhong = maLoaiPhong,
            TuNgay = tuNgay,
            DenNgay = denNgay,
            SapXep = sapXep,
            // Phân trang bằng Skip()/Take() trên truy vấn.
            KetQua = await DanhSachPhanTrang<ChiTietDatPhong>.TaoAsync(query, trang, KichThuocTrang),
            LoaiPhongs = await _context.LoaiPhongs.AsNoTracking()
                .OrderBy(l => l.TenLoai)
                .Select(l => new SelectListItem { Value = l.MaLoaiPhong.ToString(), Text = l.TenLoai })
                .ToListAsync()
        };
        return View(vm);
    }

    // ===================== CHI TIẾT =====================

    [PhanQuyen]
    public async Task<IActionResult> Details(int id)
    {
        var ct = await _context.ChiTietDatPhongs.AsNoTracking()
            .Include(c => c.Phong).ThenInclude(p => p.LoaiPhong)
            .Include(c => c.DatPhong).ThenInclude(d => d.KhachHang)
            .FirstOrDefaultAsync(c => c.MaChiTiet == id);
        if (ct == null) return NotFound();

        // Khách hàng chỉ xem được dữ liệu của chính mình (đổi mã trên URL vẫn không lộ dữ liệu người khác).
        if (!LaNhanSu() && !LaChuSoHuu(ct.DatPhong.KhachHang)) return NotFound();

        return View(ct);
    }

    // ===================== THÊM CHI TIẾT =====================

    [HttpGet]
    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> Create(int maDatPhong)
    {
        var dp = await LayDatPhongAsync(maDatPhong);
        if (dp == null) return NotFound();
        if (!DatPhongChoPhepSua(dp))
        {
            TempData["Loi"] = $"Đặt phòng #{dp.MaDatPhong} đang ở trạng thái \"{dp.TrangThai}\" nên không thể thêm phòng.";
            return RedirectToAction(nameof(KetQua), new { maDatPhong });
        }

        var vm = new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = dp.MaDatPhong,
            NgayNhan = dp.NgayNhan.Date,
            NgayTra = dp.NgayTraDuKien.Date,
            DatPhong = dp
        };
        vm.Phongs = await TaoDanhSachPhongAsync(null);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> Create(ChiTietDatPhongFormViewModel vm)
    {
        var dp = await LayDatPhongAsync(vm.MaDatPhong);
        if (dp == null) return NotFound();
        vm.DatPhong = dp;

        Phong? phong = null;
        if (ModelState.IsValid)
        {
            phong = await _context.Phongs.Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.MaPhong == vm.MaPhong);
            await KiemTraNghiepVuAsync(vm, dp, phong, 0);
        }

        if (!ModelState.IsValid || phong == null)
        {
            vm.Phongs = await TaoDanhSachPhongAsync(vm.MaPhong);
            return View(vm);
        }

        var soNgay = TinhTienHelper.TinhSoNgay(vm.NgayNhan!.Value, vm.NgayTra!.Value);
        var ct = new ChiTietDatPhong
        {
            MaDatPhong = dp.MaDatPhong,
            MaPhong = phong.MaPhong,
            NgayNhan = vm.NgayNhan.Value.Date,
            NgayTra = vm.NgayTra.Value.Date,
            DonGia = phong.DonGia,                                   // lấy theo phòng tại thời điểm đặt
            SoNgay = soNgay,                                         // hệ thống tự tính
            ThanhTien = TinhTienHelper.TinhThanhTien(phong.DonGia, soNgay),
            TrangThai = dp.TrangThai == TrangThaiDatPhong.DangXuLy
                ? TrangThaiChiTiet.DangO : TrangThaiChiTiet.ChuaNhan
        };
        _context.ChiTietDatPhongs.Add(ct);
        await _context.SaveChangesAsync();

        TempData["ThanhCong"] = $"Đã thêm phòng {phong.SoPhong} vào đặt phòng #{dp.MaDatPhong}.";
        return RedirectToAction(nameof(KetQua), new { maDatPhong = dp.MaDatPhong });
    }

    // ===================== SỬA CHI TIẾT =====================

    [HttpGet]
    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> Edit(int id)
    {
        var ct = await _context.ChiTietDatPhongs.AsNoTracking()
            .Include(c => c.Phong)
            .FirstOrDefaultAsync(c => c.MaChiTiet == id);
        if (ct == null) return NotFound();

        var dp = await LayDatPhongAsync(ct.MaDatPhong);
        if (dp == null) return NotFound();

        var loi = KiemTraChoPhepSuaXoa(ct, dp);
        if (loi != null)
        {
            TempData["Loi"] = loi;
            return RedirectToAction(nameof(KetQua), new { maDatPhong = ct.MaDatPhong });
        }

        var vm = new ChiTietDatPhongFormViewModel
        {
            MaChiTiet = ct.MaChiTiet,
            MaDatPhong = ct.MaDatPhong,
            MaPhong = ct.MaPhong,
            NgayNhan = ct.NgayNhan.Date,
            NgayTra = ct.NgayTra.Date,
            DatPhong = dp,
            Phongs = await TaoDanhSachPhongAsync(ct.MaPhong)
        };
        ViewData["TrangThaiChiTiet"] = ct.TrangThai;
        ViewData["SoPhongHienTai"] = ct.Phong.SoPhong;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> Edit(int id, ChiTietDatPhongFormViewModel vm)
    {
        if (id != vm.MaChiTiet) return NotFound();

        var ct = await _context.ChiTietDatPhongs.Include(c => c.Phong)
            .FirstOrDefaultAsync(c => c.MaChiTiet == id);
        if (ct == null) return NotFound();

        var dp = await LayDatPhongAsync(ct.MaDatPhong);
        if (dp == null) return NotFound();
        vm.MaDatPhong = ct.MaDatPhong;
        vm.DatPhong = dp;

        var loi = KiemTraChoPhepSuaXoa(ct, dp);
        if (loi != null)
        {
            TempData["Loi"] = loi;
            return RedirectToAction(nameof(KetQua), new { maDatPhong = ct.MaDatPhong });
        }

        // Phòng đang có khách thì không được đổi sang phòng khác.
        if (ct.TrangThai == TrangThaiChiTiet.DangO)
        {
            vm.MaPhong = ct.MaPhong;
            ModelState.Remove(nameof(vm.MaPhong));
        }

        Phong? phong = null;
        if (ModelState.IsValid)
        {
            phong = await _context.Phongs.Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.MaPhong == vm.MaPhong);
            await KiemTraNghiepVuAsync(vm, dp, phong, ct.MaChiTiet);
        }

        if (!ModelState.IsValid || phong == null)
        {
            vm.Phongs = await TaoDanhSachPhongAsync(vm.MaPhong);
            ViewData["TrangThaiChiTiet"] = ct.TrangThai;
            ViewData["SoPhongHienTai"] = ct.Phong.SoPhong;
            return View(vm);
        }

        // Đổi phòng thì lấy đơn giá của phòng mới; giữ phòng cũ thì giữ nguyên đơn giá đã chốt lúc đặt.
        if (phong.MaPhong != ct.MaPhong)
        {
            ct.MaPhong = phong.MaPhong;
            ct.DonGia = phong.DonGia;
        }
        ct.NgayNhan = vm.NgayNhan!.Value.Date;
        ct.NgayTra = vm.NgayTra!.Value.Date;
        ct.SoNgay = TinhTienHelper.TinhSoNgay(ct.NgayNhan, ct.NgayTra);
        ct.ThanhTien = TinhTienHelper.TinhThanhTien(ct.DonGia, ct.SoNgay);

        await _context.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã cập nhật chi tiết đặt phòng.";
        return RedirectToAction(nameof(KetQua), new { maDatPhong = ct.MaDatPhong });
    }

    // ===================== XÓA CHI TIẾT =====================

    [HttpGet]
    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> Delete(int id)
    {
        var ct = await _context.ChiTietDatPhongs.AsNoTracking()
            .Include(c => c.Phong).ThenInclude(p => p.LoaiPhong)
            .Include(c => c.DatPhong).ThenInclude(d => d.KhachHang)
            .FirstOrDefaultAsync(c => c.MaChiTiet == id);
        if (ct == null) return NotFound();

        var loi = KiemTraChoPhepXoa(ct, ct.DatPhong);
        if (loi != null)
        {
            TempData["Loi"] = loi;
            return RedirectToAction(nameof(KetQua), new { maDatPhong = ct.MaDatPhong });
        }
        return View(ct);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ct = await _context.ChiTietDatPhongs
            .Include(c => c.DatPhong)
            .FirstOrDefaultAsync(c => c.MaChiTiet == id);
        if (ct == null) return NotFound();

        var maDatPhong = ct.MaDatPhong;
        var loi = KiemTraChoPhepXoa(ct, ct.DatPhong);
        if (loi != null)
        {
            TempData["Loi"] = loi;
            return RedirectToAction(nameof(KetQua), new { maDatPhong });
        }

        _context.ChiTietDatPhongs.Remove(ct);
        await _context.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã xóa chi tiết đặt phòng.";
        return RedirectToAction(nameof(KetQua), new { maDatPhong });
    }

    // ===================== TÍNH TIỀN & CHỐT KẾT QUẢ =====================

    [HttpGet]
    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> ChotKetQua(int maDatPhong, DateTime? ngayTra)
    {
        var vm = await TaoChotKetQuaAsync(maDatPhong, (ngayTra ?? DateTime.Today).Date);
        if (vm == null) return NotFound();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
    public async Task<IActionResult> ChotKetQua(int maDatPhong, DateTime? ngayTra, string? xacNhan)
    {
        var vm = await TaoChotKetQuaAsync(maDatPhong, (ngayTra ?? DateTime.Today).Date);
        if (vm == null) return NotFound();

        if (!vm.DuDieuKien)
        {
            TempData["Loi"] = vm.LyDoKhongDuDieuKien;
            return RedirectToAction(nameof(KetQua), new { maDatPhong });
        }

        await using var giaoDich = await _context.Database.BeginTransactionAsync();

        var dp = await _context.DatPhongs
            .Include(d => d.ChiTietDatPhongs)
            .FirstAsync(d => d.MaDatPhong == maDatPhong);

        var thoiDiemCapNhat = DateTime.Now;   // thời điểm do hệ thống xác định
        var dsPhong = new List<int>();
        foreach (var ct in dp.ChiTietDatPhongs.Where(c => c.TrangThai == TrangThaiChiTiet.DangO))
        {
            ct.NgayTra = vm.NgayTra;
            ct.SoNgay = TinhTienHelper.TinhSoNgay(ct.NgayNhan, vm.NgayTra);
            ct.ThanhTien = TinhTienHelper.TinhThanhTien(ct.DonGia, ct.SoNgay);
            ct.TrangThai = TrangThaiChiTiet.DaTra;
            ct.NgayCapNhat = thoiDiemCapNhat;
            dsPhong.Add(ct.MaPhong);
        }
        dp.TrangThai = TrangThaiDatPhong.HoanThanh;
        await _context.SaveChangesAsync();

        // Trả phòng xong thì giải phóng phòng nếu không còn khách khác đang ở.
        foreach (var maPhong in dsPhong.Distinct())
        {
            var conKhach = await _context.ChiTietDatPhongs.AnyAsync(c =>
                c.MaPhong == maPhong && c.MaDatPhong != maDatPhong && c.TrangThai == TrangThaiChiTiet.DangO);
            if (conKhach) continue;

            var phong = await _context.Phongs.FindAsync(maPhong);
            if (phong != null && phong.TrangThai == TrangThaiPhong.DangSuDung)
                phong.TrangThai = TrangThaiPhong.SanSang;
        }
        await _context.SaveChangesAsync();
        await giaoDich.CommitAsync();

        TempData["ThanhCong"] = $"Đã chốt kết quả đặt phòng #{maDatPhong}. Tổng tiền {vm.TongTien:N0} đ.";
        return RedirectToAction(nameof(KetQua), new { maDatPhong });
    }

    // ===================== XEM KẾT QUẢ =====================

    [PhanQuyen]
    public async Task<IActionResult> KetQua(int maDatPhong)
    {
        var dp = await _context.DatPhongs.AsNoTracking()
            .Include(d => d.KhachHang)
            .FirstOrDefaultAsync(d => d.MaDatPhong == maDatPhong);
        if (dp == null) return NotFound();

        if (!LaNhanSu() && !LaChuSoHuu(dp.KhachHang)) return NotFound();

        var vm = new KetQuaDatPhongViewModel
        {
            DatPhong = dp,
            ChiTiets = await _context.ChiTietDatPhongs.AsNoTracking()
                .Include(c => c.Phong).ThenInclude(p => p.LoaiPhong)
                .Where(c => c.MaDatPhong == maDatPhong)
                .OrderBy(c => c.NgayNhan).ThenBy(c => c.MaChiTiet)
                .ToListAsync()
        };
        return View(vm);
    }

    [PhanQuyen(VaiTroTaiKhoan.KhachHang)]
    public async Task<IActionResult> KetQuaCuaToi(int trang = 1)
    {
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");

        var query = _context.DatPhongs.AsNoTracking()
            .Where(d => d.KhachHang.MaTaiKhoan == maTaiKhoan)   // chỉ dữ liệu của chính mình
            .OrderByDescending(d => d.NgayDat)
            .Select(d => new KetQuaCuaToiItem
            {
                DatPhong = d,
                SoPhong = d.ChiTietDatPhongs.Count(c => c.TrangThai != TrangThaiChiTiet.DaHuy),
                TongTien = d.ChiTietDatPhongs
                    .Where(c => c.TrangThai != TrangThaiChiTiet.DaHuy)
                    .Sum(c => (decimal?)c.ThanhTien) ?? 0
            });

        var ds = await DanhSachPhanTrang<KetQuaCuaToiItem>.TaoAsync(query, trang, KichThuocTrang);
        return View(ds);
    }

    // ===================== HÀM HỖ TRỢ =====================

    private bool LaNhanSu()
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        return vaiTro == VaiTroTaiKhoan.Admin || vaiTro == VaiTroTaiKhoan.NhanVien;
    }

    private bool LaChuSoHuu(KhachHang? kh)
    {
        var maTk = HttpContext.Session.GetInt32("MaTaiKhoan");
        return kh != null && maTk != null && kh.MaTaiKhoan == maTk;
    }

    private Task<DatPhong?> LayDatPhongAsync(int maDatPhong) =>
        _context.DatPhongs.AsNoTracking()
            .Include(d => d.KhachHang)
            .FirstOrDefaultAsync(d => d.MaDatPhong == maDatPhong);

    private static bool DatPhongChoPhepSua(DatPhong dp) =>
        dp.TrangThai == TrangThaiDatPhong.ChoXuLy || dp.TrangThai == TrangThaiDatPhong.DangXuLy;

    private static string? KiemTraChoPhepSuaXoa(ChiTietDatPhong ct, DatPhong dp)
    {
        if (!DatPhongChoPhepSua(dp))
            return $"Đặt phòng #{dp.MaDatPhong} đã \"{dp.TrangThai}\" nên không được thay đổi chi tiết.";
        if (ct.TrangThai == TrangThaiChiTiet.DaTra || ct.TrangThai == TrangThaiChiTiet.DaHuy)
            return $"Chi tiết đã \"{ct.TrangThai}\" nên không được thay đổi (bảo toàn lịch sử).";
        return null;
    }

    private static string? KiemTraChoPhepXoa(ChiTietDatPhong ct, DatPhong dp)
    {
        var loi = KiemTraChoPhepSuaXoa(ct, dp);
        if (loi != null) return loi;
        if (ct.TrangThai == TrangThaiChiTiet.DangO)
            return "Phòng đang có khách ở, không thể xóa chi tiết.";
        return null;
    }

    private async Task<List<SelectListItem>> TaoDanhSachPhongAsync(int? maPhongChon)
    {
        // Phòng đang bảo trì không thể kinh doanh nên không đưa vào danh sách chọn.
        var ds = await _context.Phongs.AsNoTracking()
            .Where(p => p.TrangThai != TrangThaiPhong.BaoTri || p.MaPhong == maPhongChon)
            .OrderBy(p => p.SoPhong)
            .Select(p => new { p.MaPhong, p.SoPhong, TenLoai = p.LoaiPhong.TenLoai, p.DonGia, p.TrangThai })
            .ToListAsync();

        return ds.Select(p => new SelectListItem
        {
            Value = p.MaPhong.ToString(),
            Text = $"{p.SoPhong} - {p.TenLoai} ({p.DonGia:N0} đ/ngày)",
            Selected = p.MaPhong == maPhongChon
        }).ToList();
    }

    // Các quy tắc nghiệp vụ không thể biểu diễn bằng Data Annotation.
    private async Task KiemTraNghiepVuAsync(ChiTietDatPhongFormViewModel vm, DatPhong dp, Phong? phong, int maChiTietBoQua)
    {
        if (!DatPhongChoPhepSua(dp))
        {
            ModelState.AddModelError(string.Empty,
                $"Đặt phòng đang ở trạng thái \"{dp.TrangThai}\" nên không thể thay đổi chi tiết.");
            return;
        }

        if (phong == null)
        {
            ModelState.AddModelError(nameof(vm.MaPhong), "Phòng không tồn tại.");
            return;
        }

        if (phong.TrangThai == TrangThaiPhong.BaoTri)
            ModelState.AddModelError(nameof(vm.MaPhong), $"Phòng {phong.SoPhong} đang bảo trì, không thể đặt.");

        var nhan = vm.NgayNhan!.Value.Date;
        var tra = vm.NgayTra!.Value.Date;

        // Chi tiết phải nằm trong khoảng thời gian của lần đặt.
        if (nhan < dp.NgayNhan.Date || tra > dp.NgayTraDuKien.Date)
            ModelState.AddModelError(nameof(vm.NgayNhan),
                $"Thời gian phải nằm trong khoảng đặt phòng ({dp.NgayNhan:dd/MM/yyyy} - {dp.NgayTraDuKien:dd/MM/yyyy}).");

        // Không cho đặt trùng khoảng thời gian với booking khác của cùng phòng (LINQ Any).
        var trung = await _context.ChiTietDatPhongs.AnyAsync(c =>
            c.MaPhong == phong.MaPhong
            && c.MaChiTiet != maChiTietBoQua
            && c.TrangThai != TrangThaiChiTiet.DaHuy
            && c.NgayNhan < tra && nhan < c.NgayTra);
        if (trung)
            ModelState.AddModelError(nameof(vm.MaPhong),
                $"Phòng {phong.SoPhong} đã có người đặt trong khoảng thời gian này.");

        // Cảnh báo sức chứa: tổng sức chứa các phòng của lần đặt không đủ cho số người.
        if (ModelState.IsValid)
        {
            var sucChuaCacPhongKhac = await _context.ChiTietDatPhongs
                .Where(c => c.MaDatPhong == dp.MaDatPhong
                         && c.MaChiTiet != maChiTietBoQua
                         && c.TrangThai != TrangThaiChiTiet.DaHuy)
                .SumAsync(c => (int?)c.Phong.LoaiPhong.SoNguoiToiDa) ?? 0;
            var tongSucChua = sucChuaCacPhongKhac + phong.LoaiPhong.SoNguoiToiDa;
            if (dp.SoNguoi > tongSucChua)
                TempData["CanhBao"] = $"Lưu ý: đặt phòng có {dp.SoNguoi} người nhưng tổng sức chứa các phòng hiện chỉ là {tongSucChua}.";
        }
    }

    private async Task<ChotKetQuaViewModel?> TaoChotKetQuaAsync(int maDatPhong, DateTime ngayTra)
    {
        var dp = await _context.DatPhongs.AsNoTracking()
            .Include(d => d.KhachHang)
            .Include(d => d.ChiTietDatPhongs).ThenInclude(c => c.Phong).ThenInclude(p => p.LoaiPhong)
            .FirstOrDefaultAsync(d => d.MaDatPhong == maDatPhong);
        if (dp == null) return null;

        var dangHoatDong = dp.ChiTietDatPhongs.Where(c => c.TrangThai != TrangThaiChiTiet.DaHuy).ToList();
        var vm = new ChotKetQuaViewModel { DatPhong = dp, NgayTra = ngayTra, DuDieuKien = true };

        // Chỉ giao dịch đủ điều kiện mới được cập nhật kết quả cuối cùng.
        if (dp.TrangThai != TrangThaiDatPhong.DangXuLy)
        {
            vm.DuDieuKien = false;
            vm.LyDoKhongDuDieuKien = $"Chỉ chốt kết quả khi đặt phòng ở trạng thái \"{TrangThaiDatPhong.DangXuLy}\" (hiện tại: \"{dp.TrangThai}\").";
        }
        else if (dangHoatDong.Count == 0)
        {
            vm.DuDieuKien = false;
            vm.LyDoKhongDuDieuKien = "Đặt phòng chưa có phòng nào để tính tiền.";
        }
        else if (dangHoatDong.Any(c => c.TrangThai != TrangThaiChiTiet.DangO))
        {
            vm.DuDieuKien = false;
            vm.LyDoKhongDuDieuKien = "Còn phòng chưa nhận hoặc đã xử lý riêng, cần xử lý xong trước khi chốt.";
        }
        else if (ngayTra > DateTime.Today)
        {
            vm.DuDieuKien = false;
            vm.LyDoKhongDuDieuKien = "Ngày trả thực tế không được ở tương lai.";
        }
        else if (dangHoatDong.Any(c => ngayTra < c.NgayNhan.Date))
        {
            vm.DuDieuKien = false;
            vm.LyDoKhongDuDieuKien = "Ngày trả thực tế không được trước ngày nhận phòng.";
        }

        vm.Dong = dangHoatDong.Select(c =>
        {
            var soNgay = ngayTra >= c.NgayNhan.Date ? TinhTienHelper.TinhSoNgay(c.NgayNhan, ngayTra) : 0;
            return new DongTinhTien
            {
                ChiTiet = c,
                NgayTra = ngayTra,
                SoNgay = soNgay,
                ThanhTien = TinhTienHelper.TinhThanhTien(c.DonGia, soNgay)
            };
        }).ToList();

        return vm;
    }
}
