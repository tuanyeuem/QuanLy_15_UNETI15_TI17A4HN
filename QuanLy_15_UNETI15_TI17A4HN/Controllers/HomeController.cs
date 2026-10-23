// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Trang chủ và trang lỗi của hệ thống quản lý đặt phòng khách sạn.
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Danh sách loại phòng cho ô chọn trong thanh tìm kiếm.
        ViewBag.LoaiPhongs = await _context.LoaiPhongs
            .AsNoTracking()
            .Where(l => l.TrangThai)
            .OrderBy(l => l.TenLoai)
            .ToListAsync();

        // Phòng nổi bật: mỗi loại phòng lấy một phòng còn trống có giá thấp nhất, tối đa 6 phòng.
        var phongTrong = await _context.Phongs
            .AsNoTracking()
            .Include(p => p.LoaiPhong)
            .Where(p => p.TrangThai == TrangThaiPhong.TrongRanh)
            .OrderBy(p => p.DonGia)
            .ThenBy(p => p.SoPhong)
            .Take(200)
            .ToListAsync();

        ViewBag.PhongNoiBat = phongTrong
            .GroupBy(p => p.MaLoaiPhong)
            .Select(g => g.First())
            .OrderBy(p => p.DonGia)
            .Take(6)
            .ToList();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
