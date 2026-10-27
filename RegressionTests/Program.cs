using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using System.Text;
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
var hash = MatKhauHelper.BamMatKhau("Test@123");
Check(hash != MatKhauHelper.BamMatKhau("Test@123"), "Password hashes use a random salt");
Check(MatKhauHelper.KiemTraMatKhau("Test@123", hash) && !MatKhauHelper.KiemTraMatKhau("wrong", hash),
    "Password verification accepts only the correct password");
var oldHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("Test@123")));
Check(MatKhauHelper.KiemTraMatKhau("Test@123", oldHash, out var upgrade) && upgrade,
    "Legacy SHA-256 passwords remain valid and request an upgrade");
Check(!MatKhauHelper.KiemTraMatKhau("Test@123", "invalid-hash"), "Malformed password hashes fail safely");
await using var seed = NewDb();
try
{
    await seed.Database.EnsureCreatedAsync();
    var type = new LoaiPhong { TenLoai = "Test", SoNguoiToiDa = 2 };
    var rooms = Enumerable.Range(1, 12).Select(i => new Phong
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
            TrangThai = TrangThaiDatPhong.DaDuyet
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
        await Setup(new DatPhongController(db)).NhanPhongPost(moved.MaDatPhong);
        var actual = await db.DatPhongs.Include(d => d.ChiTietDatPhongs).SingleAsync(d => d.MaDatPhong == moved.MaDatPhong);
        Check(actual.MaPhong == rooms[1].MaPhong && actual.ChiTietDatPhongs.Count == 1
            && actual.ChiTietDatPhongs.Single().TrangThai == TrangThaiChiTiet.DangO,
            "Check-in keeps only the replacement room");
    }

    // Existing inconsistent data: primary room differs from the sole detail room.
    var legacyMoved = await Booking(rooms[1], today, today.AddDays(3), true);
    legacyMoved.ChiTietDatPhongs.Single().Phong = rooms[0];
    await seed.SaveChangesAsync();
    await using (var db = NewDb())
    {
        await Setup(new DatPhongController(db)).NhanPhongPost(legacyMoved.MaDatPhong);
        var actual = await db.DatPhongs.Include(d => d.ChiTietDatPhongs).SingleAsync(d => d.MaDatPhong == legacyMoved.MaDatPhong);
        Check(actual.TrangThai == TrangThaiDatPhong.DaNhanPhong && actual.MaPhong == rooms[0].MaPhong
            && actual.ChiTietDatPhongs.Count == 1,
            "Check-in repairs a stale primary room without adding a second detail");
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

    var future = await Booking(rooms[4], today.AddDays(1), today.AddDays(3), true);
    await using (var db = NewDb())
    {
        await Setup(new DatPhongController(db)).NhanPhongPost(future.MaDatPhong);
        Check((await db.DatPhongs.FindAsync(future.MaDatPhong))!.TrangThai == TrangThaiDatPhong.DaDuyet,
            "Check-in before the reserved arrival date is rejected");
    }

    var late = await Booking(rooms[5], today.AddDays(-2), today.AddDays(2), true);
    late.ChiTietDatPhongs.Single().DonGia = 0;
    await seed.SaveChangesAsync();
    await using (var db = NewDb())
    {
        await Setup(new DatPhongController(db)).NhanPhongPost(late.MaDatPhong);
        var actual = await db.ChiTietDatPhongs.SingleAsync(c => c.MaDatPhong == late.MaDatPhong);
        Check(actual.TrangThai == TrangThaiChiTiet.DangO && actual.NgayNhan == today
            && actual.SoNgay == 2 && actual.ThanhTien == 0 && actual.DonGia == 0,
            "Late check-in uses actual arrival and preserves the agreed zero rate");
        Check(await TrangThaiPhongHelper.KiemTraAsync(db, rooms[5].MaPhong, TrangThaiPhong.TrongRanh) != null,
            "An occupied room cannot be marked empty");
        var controller = Setup(new PhongController(db, NullLogger<PhongController>.Instance));
        await controller.CapNhatTrangThai(rooms[5].MaPhong, TrangThaiPhong.BaoTri);
        Check((await db.Phongs.FindAsync(rooms[5].MaPhong))!.TrangThai == TrangThaiPhong.DangSuDung,
            "The room status endpoint rejects maintenance while guests are staying");
    }

    var expired = await Booking(rooms[6], today.AddDays(-2), today, true);
    await using (var db = NewDb())
    {
        await Setup(new DatPhongController(db)).NhanPhongPost(expired.MaDatPhong);
        Check((await db.DatPhongs.FindAsync(expired.MaDatPhong))!.TrangThai == TrangThaiDatPhong.DaDuyet,
            "Check-in on the planned checkout date is rejected");
        Check(await TrangThaiPhongHelper.KiemTraAsync(db, rooms[4].MaPhong, TrangThaiPhong.BaoTri) != null,
            "Maintenance is rejected while a future reservation remains");
    }

    var multi = await Booking(rooms[7], today.AddDays(20), today.AddDays(22), true);
    var primaryDetail = multi.ChiTietDatPhongs.Single().MaChiTiet;
    await using (var db = NewDb())
    {
        var controller = Setup(new ChiTietDatPhongController(db));
        await controller.Create(new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = multi.MaDatPhong, MaPhong = rooms[8].MaPhong,
            NgayNhan = today.AddDays(20), NgayTra = today.AddDays(22)
        });
        await controller.DeleteConfirmed(primaryDetail);
        var booking = await db.DatPhongs.Include(d => d.ChiTietDatPhongs).SingleAsync(d => d.MaDatPhong == multi.MaDatPhong);
        Check(booking.MaPhong == rooms[8].MaPhong && booking.ChiTietDatPhongs.Count == 1,
            "Deleting the primary room selects the remaining room as representative");
        await controller.DeleteConfirmed(booking.ChiTietDatPhongs.Single().MaChiTiet);
        Check(await db.ChiTietDatPhongs.CountAsync(c => c.MaDatPhong == multi.MaDatPhong) == 1,
            "The last room cannot be deleted and resurrected through legacy fallback");
    }

    var legacyAdd = await Booking(rooms[9], today.AddDays(30), today.AddDays(32));
    await using (var db = NewDb())
    {
        await Setup(new ChiTietDatPhongController(db)).Create(new ChiTietDatPhongFormViewModel
        {
            MaDatPhong = legacyAdd.MaDatPhong, MaPhong = rooms[10].MaPhong,
            NgayNhan = today.AddDays(30), NgayTra = today.AddDays(32)
        });
        Check(await db.ChiTietDatPhongs.CountAsync(c => c.MaDatPhong == legacyAdd.MaDatPhong) == 2
            && await LichPhongHelper.CoTrungAsync(db, rooms[9].MaPhong, today.AddDays(30), today.AddDays(31)),
            "Adding a room to a legacy reservation preserves its original room and occupied dates");
        Check(!await LichPhongHelper.CoTrungAsync(db, rooms[9].MaPhong, today.AddDays(32), today.AddDays(33)),
            "Adjacent stays do not overlap");
    }

    var overCapacity = await Booking(rooms[11], today, today.AddDays(2), true);
    overCapacity.SoNguoi = 3;
    await seed.SaveChangesAsync();
    await using (var db = NewDb())
    {
        await Setup(new DatPhongController(db)).NhanPhongPost(overCapacity.MaDatPhong);
        Check((await db.DatPhongs.FindAsync(overCapacity.MaDatPhong))!.TrangThai == TrangThaiDatPhong.DaDuyet,
            "Check-in revalidates room capacity after approval");
    }

    await using (var db = NewDb())
    {
        var room = await db.Phongs.FindAsync(rooms[11].MaPhong);
        room!.TrangThai = TrangThaiPhong.KhongKinhDoanh;
        await db.SaveChangesAsync();
        var statistics = Setup(new ThongKeController(db));
        var result = (ViewResult)await statistics.Index(today.Year);
        var report = (ThongKeViewModel)result.Model!;
        Check(report.PhongKinhDoanh == rooms.Length - 1, "Occupancy denominator excludes rooms not in business");
        var emptyYear = (ThongKeViewModel)((ViewResult)await statistics.Index(today.Year - 5)).Model!;
        Check(emptyYear.PhongDatNhieuNhat.Count == 0, "Top rooms respect the selected year");
        var dashboard = (DashboardViewModel)((ViewResult)await statistics.Dashboard()).Model!;
        Check(dashboard.DangO == 2, "Dashboard includes checked-in bookings as a distinct status");
        var search = Setup(new PhongController(db, NullLogger<PhongController>.Instance));
        await search.TimKiem(null, null, null, TrangThaiPhong.KhongKinhDoanh, null, null,
            today.AddDays(100), today.AddDays(101), null);
        Check((int)search.ViewBag.TongSoPhong == 0, "Date search never offers non-business rooms even with a status filter");
    }
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
