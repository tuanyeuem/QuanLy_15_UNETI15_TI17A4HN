using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Controllers;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
using QuanLy_15_UNETI15_TI17A4HN.ViewModels;

// Database riêng cho mỗi lần chạy; không truy cập database ứng dụng.
var database = "QLKS_Regression_" + Guid.NewGuid().ToString("N");
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Database={database};Trusted_Connection=True;TrustServerCertificate=True")
    .Options;
AppDbContext NewDb() => new(options);
T Setup<T>(T controller) where T : Controller
{
    var http = new DefaultHttpContext();
    controller.ControllerContext = new ControllerContext { HttpContext = http };
    controller.TempData = new TempDataDictionary(http, new MemoryTempData());
    return controller;
}
void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    Console.WriteLine("PASS: " + message);
}
await using var seed = NewDb();
try
{
    await seed.Database.EnsureCreatedAsync();
    var type = new LoaiPhong { TenLoai = "Test", SoNguoiToiDa = 2 };
    var rooms = Enumerable.Range(1, 4).Select(i => new Phong
    {
        SoPhong = i.ToString(), LoaiPhong = type, DonGia = 100, Tang = 1
    }).ToArray();
    var guest = new KhachHang { HoTen = "Regression" };
    seed.AddRange(rooms);
    seed.Add(guest);
    await seed.SaveChangesAsync();
    var today = DateTime.Today;
    async Task<DatPhong> Booking(Phong room, DateTime start, DateTime end, bool detail = false)
    {
        var booking = new DatPhong
        {
            Phong = room, KhachHang = guest, NgayNhan = start, NgayTraDuKien = end,
            TrangThai = TrangThaiDatPhong.ChoXuLy
        };
        if (detail) booking.ChiTietDatPhongs.Add(new ChiTietDatPhong
        {
            Phong = room, NgayNhan = start, NgayTra = end, DonGia = 100,
            SoNgay = (end - start).Days, ThanhTien = 100 * (end - start).Days
        });
        seed.Add(booking);
        await seed.SaveChangesAsync();
        return booking;
    }

    var moved = await Booking(rooms[0], today, today.AddDays(3), true);
    var detailId = moved.ChiTietDatPhongs.Single().MaChiTiet;
    await using (var db = NewDb())
    {
        var result = await Setup(new ChiTietDatPhongController(db)).Edit(detailId,
            new ChiTietDatPhongFormViewModel
            {
                MaChiTiet = detailId, MaDatPhong = moved.MaDatPhong, MaPhong = rooms[1].MaPhong,
                NgayNhan = today, NgayTra = today.AddDays(3)
            });
        Check(result is RedirectToActionResult, "Room change succeeds");
    }
    await using (var db = NewDb())
    {
        var actual = await db.DatPhongs.Include(d => d.ChiTietDatPhongs).SingleAsync(d => d.MaDatPhong == moved.MaDatPhong);
        Check(actual.MaPhong == rooms[1].MaPhong && actual.ChiTietDatPhongs.Count == 1
            && actual.ChiTietDatPhongs.Single().MaPhong == rooms[1].MaPhong,
            "Room change synchronizes the primary room without adding a second detail");
        actual.TrangThai = TrangThaiDatPhong.DangXuLy;
        actual.ChiTietDatPhongs.Single().TrangThai = TrangThaiChiTiet.DangO;
        await db.SaveChangesAsync();
    }

    await using (var db = NewDb())
        await Setup(new ChiTietDatPhongController(db)).ChotKetQua(moved.MaDatPhong, today, "1");
    var next = await Booking(rooms[1], today.AddDays(1), today.AddDays(2));
    await using (var db = NewDb())
    {
        var result = await Setup(new ChiTietDatPhongController(db)).Create(new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = next.MaDatPhong, MaPhong = rooms[1].MaPhong,
            NgayNhan = today.AddDays(1), NgayTra = today.AddDays(2)
        });
        Check(result is RedirectToActionResult, "Early checkout releases the unused booked dates");
    }

    // Legacy booking without details must still reserve its room.
    await Booking(rooms[2], today, today.AddDays(3));
    var blocked = await Booking(rooms[0], today, today.AddDays(3));
    await using (var db = NewDb())
    {
        var result = await Setup(new ChiTietDatPhongController(db)).Create(new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = blocked.MaDatPhong, MaPhong = rooms[2].MaPhong,
            NgayNhan = today, NgayTra = today.AddDays(2)
        });
        Check(result is ViewResult && !await db.ChiTietDatPhongs.AnyAsync(c => c.MaDatPhong == blocked.MaDatPhong),
            "Legacy primary-room reservation still blocks overlaps");
    }

    var concurrentA = await Booking(rooms[0], today.AddDays(10), today.AddDays(12));
    var concurrentB = await Booking(rooms[1], today.AddDays(10), today.AddDays(12));
    async Task<IActionResult> Reserve(int id)
    {
        await using var db = NewDb();
        return await Setup(new ChiTietDatPhongController(db)).Create(new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = id, MaPhong = rooms[3].MaPhong,
            NgayNhan = today.AddDays(10), NgayTra = today.AddDays(12)
        });
    }
    // Hold the shared lock so both competing requests queue before either can read.
    await using (var gateDb = NewDb())
    {
        await using var gate = await GiaoDichLichPhong.BatDauAsync(gateDb);
        var a = Reserve(concurrentA.MaDatPhong);
        var b = Reserve(concurrentB.MaDatPhong);
        await Task.Delay(300);
        Check(!a.IsCompleted && !b.IsCompleted, "Concurrent requests wait for the database lock");
        await gate.CommitAsync();
        var results = await Task.WhenAll(a, b);
        Check(results.Count(r => r is RedirectToActionResult) == 1 && results.Count(r => r is ViewResult) == 1,
            "Exactly one overlapping reservation succeeds");
    }
    await using (var db = NewDb())
        Check(await db.ChiTietDatPhongs.CountAsync(c => c.MaPhong == rooms[3].MaPhong) == 1,
            "Database contains only one reservation for the contested room");
}
finally
{
    await seed.Database.EnsureDeletedAsync();
}

sealed class MemoryTempData : ITempDataProvider
{
    public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
    public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
}
