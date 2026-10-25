// Họ và tên: Thành viên Module 2 (Git: tkv261005)
// Mã sinh viên: (bổ sung MSSV khi nộp bài)
// Nội dung thực hiện: Module 2 - Tải lên/xóa ảnh phòng, gộp giao diện vào trang sửa phòng.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

// Tải lên hoặc xóa ảnh của từng phòng. Giao diện nằm ở trang Sửa phòng (Phong/Edit).
// Chỉ Admin và Nhân viên được thao tác.
[PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
public class PhongAnhController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public PhongAnhController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // Trang riêng trước đây đã được gộp vào trang Sửa phòng.
    [HttpGet]
    public IActionResult Doi(int id) => RedirectToAction("Edit", "Phong", new { id });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Doi(int id, IFormFile? tepAnh)
    {
        var phong = await TimPhongAsync(id);
        if (phong == null) return NotFound();

        if (tepAnh == null || tepAnh.Length == 0)
            return KetQua(phong, false, "Vui lòng chọn một tệp ảnh.");
        if (tepAnh.Length > PhongAnhHelper.KichThuocToiDa)
            return KetQua(phong, false, "Ảnh quá lớn. Dung lượng tối đa là 5 MB.");

        string? duoi;
        await using (var luongDoc = tepAnh.OpenReadStream())
        {
            duoi = PhongAnhHelper.NhanDangDuoi(luongDoc);
        }
        if (duoi == null)
            return KetQua(phong, false, "Chỉ chấp nhận ảnh JPG, PNG hoặc WEBP.");

        Directory.CreateDirectory(PhongAnhHelper.ThuMucVatLy(_env));
        PhongAnhHelper.XoaAnh(_env, id);
        var duongDan = Path.Combine(PhongAnhHelper.ThuMucVatLy(_env), $"phong-{id}{duoi}");
        await using (var luongGhi = System.IO.File.Create(duongDan))
        {
            await tepAnh.CopyToAsync(luongGhi);
        }

        return KetQua(phong, true, $"Đã cập nhật ảnh cho phòng {phong.SoPhong}.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int id)
    {
        var phong = await TimPhongAsync(id);
        if (phong == null) return NotFound();

        PhongAnhHelper.XoaAnh(_env, id);
        return KetQua(phong, true, $"Phòng {phong.SoPhong} đã quay về ảnh minh họa mặc định.");
    }

    private bool LaAjax() => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

    // AJAX: trả JSON để trang Sửa cập nhật ảnh tại chỗ. Không phải AJAX: báo bằng TempData rồi quay về trang Sửa.
    private IActionResult KetQua(Phong phong, bool thanhCong, string noiDung)
    {
        if (LaAjax())
        {
            return StatusCode(thanhCong ? 200 : 400, new
            {
                ok = thanhCong,
                message = noiDung,
                url = PhongAnhHelper.Url(_env, phong),
                coAnhRieng = PhongAnhHelper.AnhRieng(_env, phong.MaPhong) != null
            });
        }
        TempData[thanhCong ? "ThanhCong" : "Loi"] = noiDung;
        return RedirectToAction("Edit", "Phong", new { id = phong.MaPhong });
    }

    private Task<Phong?> TimPhongAsync(int id)
        => _context.Phongs.AsNoTracking()
            .Include(p => p.LoaiPhong)
            .FirstOrDefaultAsync(p => p.MaPhong == id);
}
