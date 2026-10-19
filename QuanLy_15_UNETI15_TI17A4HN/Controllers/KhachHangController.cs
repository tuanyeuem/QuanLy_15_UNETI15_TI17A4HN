// Họ và tên: Nguyễn Đình Quang
// Mã sinh viên: 23103100259
// Nội dung thực hiện: Module 3 - Hồ sơ khách hàng, tạo, theo dõi và hủy đặt phòng cá nhân.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

[PhanQuyen(VaiTroTaiKhoan.KhachHang)]
public class KhachHangController : Controller
{
    private readonly AppDbContext _db;

    public KhachHangController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> HoSo()
    {
        var khachHang = await LayHoSoAsync();
        return View(MapHoSo(khachHang));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HoSo(CapNhatHoSoViewModel model)
    {
        var khachHang = await LayHoSoAsync();
        if (model.NgaySinh.HasValue && model.NgaySinh.Value.Date > DateTime.Today)
            ModelState.AddModelError(nameof(model.NgaySinh), "Ngày sinh không thể ở tương lai.");

        var cccd = Clean(model.CCCD);
        if (cccd != null && await _db.KhachHangs.AnyAsync(k =>
                k.CCCD == cccd && k.MaKhachHang != khachHang.MaKhachHang))
            ModelState.AddModelError(nameof(model.CCCD), "CCCD đã được sử dụng cho hồ sơ khác.");

        if (!ModelState.IsValid) return View(model);

        khachHang.HoTen = model.HoTen.Trim();
        khachHang.NgaySinh = model.NgaySinh?.Date;
        khachHang.GioiTinh = Clean(model.GioiTinh);
        khachHang.CCCD = cccd;
        khachHang.SoDienThoai = Clean(model.SoDienThoai);
        khachHang.Email = Clean(model.Email);
        khachHang.DiaChi = Clean(model.DiaChi);
        khachHang.QuocTich = Clean(model.QuocTich);
        await _db.SaveChangesAsync();

        TempData["ThanhCong"] = "Đã cập nhật hồ sơ cá nhân.";
        return RedirectToAction(nameof(HoSo));
    }

    [HttpGet]
    public async Task<IActionResult> DatPhong(DateTime? ngayNhan, DateTime? ngayTra)
    {
        var model = new TaoDatPhongViewModel
        {
            NgayNhan = ngayNhan?.Date ?? DateTime.Today.AddDays(1),
            NgayTraDuKien = ngayTra?.Date ?? DateTime.Today.AddDays(2)
        };
        await NapDanhSachPhongAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatPhong(TaoDatPhongViewModel model)
    {
        var khachHang = await LayHoSoAsync();
        var nhan = model.NgayNhan.Date;
        var tra = model.NgayTraDuKien.Date;

        if (!khachHang.TrangThai)
            ModelState.AddModelError(string.Empty, "Hồ sơ khách hàng đang bị khóa, không thể đặt phòng.");
        if (nhan < DateTime.Today)
            ModelState.AddModelError(nameof(model.NgayNhan), "Ngày nhận không được ở trong quá khứ.");
        if (tra <= nhan)
            ModelState.AddModelError(nameof(model.NgayTraDuKien), "Ngày trả phải sau ngày nhận.");

        Phong? phong = null;
        if (model.MaPhong.HasValue)
        {
            phong = await _db.Phongs.Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.MaPhong == model.MaPhong.Value);
            if (phong == null || !PhongCoTheDat(phong.TrangThai))
                ModelState.AddModelError(nameof(model.MaPhong), "Phòng không tồn tại hoặc hiện không kinh doanh.");
            else if (model.SoNguoi > phong.LoaiPhong.SoNguoiToiDa)
                ModelState.AddModelError(nameof(model.SoNguoi), $"Loại phòng này tối đa {phong.LoaiPhong.SoNguoiToiDa} người.");
            else if (await BiTrungLichAsync(phong.MaPhong, nhan, tra))
                ModelState.AddModelError(nameof(model.MaPhong), "Phòng đã có đặt phòng trong khoảng thời gian này.");
        }
        else
        {
            ModelState.AddModelError(nameof(model.MaPhong), "Vui lòng chọn phòng.");
        }

        if (!ModelState.IsValid)
        {
            await NapDanhSachPhongAsync(model);
            return View(model);
        }

        var datPhong = new DatPhong
        {
            MaKhachHang = khachHang.MaKhachHang,
            MaPhong = phong!.MaPhong,
            NgayDat = DateTime.Now,
            NgayNhan = nhan,
            NgayTraDuKien = tra,
            SoNguoi = model.SoNguoi,
            TienCoc = model.TienCoc,
            GhiChu = Clean(model.GhiChu),
            TrangThai = TrangThaiDatPhong.ChoDuyet
        };

        _db.DatPhongs.Add(datPhong);
        var soNgay = TinhTienHelper.TinhSoNgay(nhan, tra);
        _db.ChiTietDatPhongs.Add(new ChiTietDatPhong
        {
            DatPhong = datPhong,
            Phong = phong!,
            NgayNhan = nhan,
            NgayTra = tra,
            DonGia = phong!.DonGia,
            SoNgay = soNgay,
            ThanhTien = TinhTienHelper.TinhThanhTien(phong.DonGia, soNgay),
            TrangThai = TrangThaiChiTiet.ChuaNhan
        });
        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = $"Đã gửi yêu cầu đặt phòng #{datPhong.MaDatPhong}. Vui lòng theo dõi trạng thái xử lý.";
        return RedirectToAction(nameof(TheoDoi));
    }

    [HttpGet]
    public async Task<IActionResult> TheoDoi()
    {
        var khachHang = await LayHoSoAsync();
        var datPhongs = await _db.DatPhongs.AsNoTracking()
            .Include(d => d.Phong).ThenInclude(p => p.LoaiPhong)
            .Include(d => d.ChiTietDatPhongs)
            .Where(d => d.MaKhachHang == khachHang.MaKhachHang)
            .OrderByDescending(d => d.NgayDat)
            .ToListAsync();
        return View(datPhongs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int id)
    {
        var khachHang = await LayHoSoAsync();
        var datPhong = await _db.DatPhongs.Include(d => d.ChiTietDatPhongs)
            .FirstOrDefaultAsync(d => d.MaDatPhong == id && d.MaKhachHang == khachHang.MaKhachHang);
        if (datPhong == null) return NotFound();

        if (datPhong.TrangThai is not (TrangThaiDatPhong.ChoDuyet or TrangThaiDatPhong.DaDuyet))
        {
            TempData["Loi"] = "Chỉ có thể hủy đặt phòng đang chờ duyệt hoặc đã duyệt.";
            return RedirectToAction(nameof(TheoDoi));
        }

        datPhong.TrangThai = TrangThaiDatPhong.Huy;
        datPhong.LyDoHuy = "Khách hàng tự hủy đặt phòng.";
        foreach (var chiTiet in datPhong.ChiTietDatPhongs.Where(c => c.TrangThai == TrangThaiChiTiet.ChuaNhan))
        {
            chiTiet.TrangThai = TrangThaiChiTiet.DaHuy;
            chiTiet.NgayCapNhat = DateTime.Now;
        }
        await _db.SaveChangesAsync();
        TempData["ThanhCong"] = "Đã hủy đặt phòng.";
        return RedirectToAction(nameof(TheoDoi));
    }

    private async Task<KhachHang> LayHoSoAsync()
    {
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        if (!maTaiKhoan.HasValue) throw new InvalidOperationException("Phiên đăng nhập không hợp lệ.");

        var taiKhoan = await _db.TaiKhoans.AsNoTracking()
            .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan.Value && t.TrangThai);
        if (taiKhoan == null)
            throw new InvalidOperationException("Tài khoản không tồn tại hoặc đã bị khóa.");

        var khachHang = await _db.KhachHangs
            .FirstOrDefaultAsync(k => k.MaTaiKhoan == taiKhoan.MaTaiKhoan);
        if (khachHang != null) return khachHang;

        khachHang = new KhachHang
        {
            MaTaiKhoan = taiKhoan.MaTaiKhoan,
            HoTen = taiKhoan.HoTen,
            Email = taiKhoan.Email,
            TrangThai = true,
            QuocTich = "Việt Nam"
        };
        _db.KhachHangs.Add(khachHang);
        await _db.SaveChangesAsync();
        return khachHang;
    }

    private async Task NapDanhSachPhongAsync(TaoDatPhongViewModel model)
    {
        var nhan = model.NgayNhan.Date;
        var tra = model.NgayTraDuKien.Date;
        var query = _db.Phongs.AsNoTracking().Include(p => p.LoaiPhong)
            .Where(p => p.LoaiPhong.TrangThai && p.TrangThai == TrangThaiPhong.TrongRanh);

        if (tra > nhan)
            query = query.Where(p => !_db.DatPhongs.Any(d => d.MaPhong == p.MaPhong &&
                d.TrangThai != TrangThaiDatPhong.Huy &&
                d.TrangThai != TrangThaiDatPhong.TuChoi &&
                d.TrangThai != TrangThaiDatPhong.DaTraPhong &&
                d.NgayNhan < tra && d.NgayTraDuKien > nhan));

        model.DanhSachPhong = await query.OrderBy(p => p.SoPhong)
            .Select(p => new SelectListItem
            {
                Value = p.MaPhong.ToString(),
                Text = p.SoPhong + " - " + p.LoaiPhong.TenLoai + " (" + p.DonGia + " đ/đêm, tối đa " + p.LoaiPhong.SoNguoiToiDa + " người)"
            }).ToListAsync();
    }

    private Task<bool> BiTrungLichAsync(int maPhong, DateTime ngayNhan, DateTime ngayTra) =>
        _db.DatPhongs.AnyAsync(d => d.MaPhong == maPhong &&
            d.TrangThai != TrangThaiDatPhong.Huy &&
            d.TrangThai != TrangThaiDatPhong.TuChoi &&
            d.TrangThai != TrangThaiDatPhong.DaTraPhong &&
            d.NgayNhan < ngayTra && d.NgayTraDuKien > ngayNhan);

    private static bool PhongCoTheDat(string trangThai) => trangThai == TrangThaiPhong.TrongRanh;

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static CapNhatHoSoViewModel MapHoSo(KhachHang khachHang) => new()
    {
        HoTen = khachHang.HoTen,
        NgaySinh = khachHang.NgaySinh,
        GioiTinh = khachHang.GioiTinh,
        CCCD = khachHang.CCCD,
        SoDienThoai = khachHang.SoDienThoai,
        Email = khachHang.Email,
        DiaChi = khachHang.DiaChi,
        QuocTich = khachHang.QuocTich
    };
}
