using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Models;

namespace QuanLy_15_UNETI15_TI17A4HN.Controllers
{
    public class PhongController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PhongController> _logger;

        public PhongController(
            AppDbContext context,
            ILogger<PhongController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var dsPhong = await _context.Phongs
                .ToListAsync();

            return View(dsPhong);
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var phong = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.MaPhong == id);

            if (phong == null)
                return NotFound();

            return View(phong);
        }
        public IActionResult Create()
        {
            LoadLoaiPhong();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phong phong)
        {
            _logger.LogInformation(
                "Bắt đầu thêm phòng. MaPhong={MaPhong}, SoPhong={SoPhong}, MaLoaiPhong={MaLoaiPhong}",
                phong.MaPhong, phong.SoPhong, phong.MaLoaiPhong);

            try
            {
                phong.MaPhong = phong.MaPhong?.Trim();
                phong.SoPhong = phong.SoPhong?.Trim();
                phong.MaLoaiPhong = phong.MaLoaiPhong?.Trim();

                if (string.IsNullOrWhiteSpace(phong.MaPhong))
                {
                    ModelState.AddModelError(
                        nameof(phong.MaPhong), "Mã phòng không được để trống.");
                }

                if (string.IsNullOrWhiteSpace(phong.SoPhong))
                {
                    ModelState.AddModelError(
                        nameof(phong.SoPhong), "Số phòng không được để trống.");
                }

                if (string.IsNullOrWhiteSpace(phong.MaLoaiPhong))
                {
                    ModelState.AddModelError(
                        nameof(phong.MaLoaiPhong), "Vui lòng chọn loại phòng.");
                }

                if (!string.IsNullOrWhiteSpace(phong.MaPhong))
                {
                    bool maPhongTonTai = await _context.Phongs
                        .AnyAsync(p => p.MaPhong == phong.MaPhong);

                    if (maPhongTonTai)
                    {
                        ModelState.AddModelError(
                            nameof(phong.MaPhong), "Mã phòng đã tồn tại.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(phong.SoPhong))
                {
                    bool soPhongTonTai = await _context.Phongs
                        .AnyAsync(p => p.SoPhong == phong.SoPhong);

                    if (soPhongTonTai)
                    {
                        ModelState.AddModelError(
                            nameof(phong.SoPhong), "Số phòng đã tồn tại.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(phong.MaLoaiPhong))
                {
                    bool loaiPhongTonTai = await _context.LoaiPhongs
                        .AnyAsync(l => l.MaLoaiPhong == phong.MaLoaiPhong);

                    if (!loaiPhongTonTai)
                    {
                        ModelState.AddModelError(
                            nameof(phong.MaLoaiPhong),
                            "Loại phòng không tồn tại. Vui lòng chọn lại.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e =>
                            $"{x.Key}: {e.ErrorMessage}"));

                    _logger.LogWarning(
                        "Dữ liệu thêm phòng không hợp lệ: {Errors}",
                        string.Join(" | ", errors));

                    TempData["Error"] =
                        "Thêm phòng thất bại. Vui lòng kiểm tra lại thông tin.";

                    LoadLoaiPhong(phong.MaLoaiPhong);
                    return View(phong);
                }

                _context.Phongs.Add(phong);

                _logger.LogInformation(
                    "Bắt đầu lưu phòng {MaPhong} vào database.",
                    phong.MaPhong);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Thêm phòng {MaPhong} thành công.",
                    phong.MaPhong);

                TempData["Success"] = "Thêm phòng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi database khi thêm phòng {MaPhong}. InnerException: {InnerMessage}",
                    phong.MaPhong,
                    ex.InnerException?.Message);

                _context.Entry(phong).State = EntityState.Detached;

                TempData["Error"] =
                    "Không thể lưu phòng vào database. Hãy kiểm tra log máy chủ.";

                LoadLoaiPhong(phong.MaLoaiPhong);
                return View(phong);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi không xác định khi thêm phòng {MaPhong}.",
                    phong.MaPhong);

                _context.Entry(phong).State = EntityState.Detached;

                TempData["Error"] =
                    "Đã xảy ra lỗi khi thêm phòng. Vui lòng thử lại.";

                LoadLoaiPhong(phong.MaLoaiPhong);
                return View(phong);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Edit GET: Không nhận được mã phòng.");
                return NotFound();
            }

            try
            {
                var phong = await _context.Phongs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.MaPhong == id);

                if (phong == null)
                {
                    _logger.LogWarning(
                        "Edit GET: Không tìm thấy phòng {MaPhong}.", id);
                    return NotFound();
                }

                LoadLoaiPhong(phong.MaLoaiPhong);

                return View(phong);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Lỗi khi tải thông tin phòng {MaPhong}.", id);

                return Problem(
                    title: "Không thể tải thông tin phòng.",
                    statusCode: 500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, Phong phong)
        {
            if (string.IsNullOrWhiteSpace(id) || id != phong.MaPhong)
            {
                _logger.LogWarning(
                    "Edit POST: Mã phòng không hợp lệ. Id={Id}, MaPhong={MaPhong}",
                    id, phong.MaPhong);

                return NotFound();
            }

            try
            {
                phong.MaPhong = phong.MaPhong?.Trim()!;
                phong.SoPhong = phong.SoPhong?.Trim()!;
                phong.MaLoaiPhong = phong.MaLoaiPhong?.Trim()!;

                if (!string.IsNullOrWhiteSpace(phong.SoPhong))
                {
                    bool soPhongTonTai = await _context.Phongs
                        .AnyAsync(p =>
                            p.SoPhong == phong.SoPhong &&
                            p.MaPhong != phong.MaPhong);

                    if (soPhongTonTai)
                    {
                        ModelState.AddModelError(
                            nameof(phong.SoPhong),
                            "Số phòng đã được sử dụng bởi phòng khác.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(phong.MaLoaiPhong))
                {
                    bool loaiPhongTonTai = await _context.LoaiPhongs
                        .AnyAsync(l => l.MaLoaiPhong == phong.MaLoaiPhong);

                    if (!loaiPhongTonTai)
                    {
                        ModelState.AddModelError(
                            nameof(phong.MaLoaiPhong),
                            "Loại phòng không tồn tại. Vui lòng chọn lại.");
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        nameof(phong.MaLoaiPhong),
                        "Vui lòng chọn loại phòng.");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value != null &&
                                    x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e =>
                            $"{x.Key}: {e.ErrorMessage}"));

                    _logger.LogWarning(
                        "Edit: Dữ liệu phòng {MaPhong} không hợp lệ: {Errors}",
                        phong.MaPhong,
                        string.Join(" | ", errors));

                    TempData["Error"] =
                        "Cập nhật phòng thất bại. Vui lòng kiểm tra lại thông tin.";

                    LoadLoaiPhong(phong.MaLoaiPhong);
                    return View(phong);
                }

                var phongDb = await _context.Phongs
                    .FirstOrDefaultAsync(p => p.MaPhong == id);

                if (phongDb == null)
                {
                    _logger.LogWarning(
                        "Edit: Phòng {MaPhong} không còn tồn tại.", id);

                    return NotFound();
                }

                phongDb.SoPhong = phong.SoPhong;
                phongDb.MaLoaiPhong = phong.MaLoaiPhong;
                phongDb.Tang = phong.Tang;
                phongDb.HuongPhong = phong.HuongPhong;
                phongDb.TienNghi = phong.TienNghi;
                phongDb.DonGia = phong.DonGia;
                phongDb.TrangThai = phong.TrangThai;
                phongDb.GhiChu = phong.GhiChu;
                phongDb.NgayBaoTri = phong.NgayBaoTri;
                phongDb.GhiChuBaoTri = phong.GhiChuBaoTri;

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Cập nhật phòng {MaPhong} thành công.", id);

                TempData["Success"] = "Cập nhật phòng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(
                    ex, "Xung đột đồng thời khi sửa phòng {MaPhong}.", id);

                bool phongConTonTai = await _context.Phongs
                    .AnyAsync(p => p.MaPhong == id);

                if (!phongConTonTai)
                    return NotFound();

                TempData["Error"] =
                    "Dữ liệu phòng đã thay đổi. Vui lòng tải lại và thử lại.";

                LoadLoaiPhong(phong.MaLoaiPhong);
                return View(phong);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi database khi sửa phòng {MaPhong}. Chi tiết: {InnerMessage}",
                    id,
                    ex.InnerException?.Message);

                TempData["Error"] =
                    "Không thể cập nhật phòng vào database. Vui lòng kiểm tra dữ liệu.";

                LoadLoaiPhong(phong.MaLoaiPhong);
                return View(phong);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Lỗi không xác định khi sửa phòng {MaPhong}.", id);

                TempData["Error"] =
                    "Đã xảy ra lỗi khi cập nhật phòng.";

                LoadLoaiPhong(phong.MaLoaiPhong);
                return View(phong);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var phong = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.MaPhong == id);

            if (phong == null)
                return NotFound();

            return View(phong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Mã phòng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var phong = await _context.Phongs
                    .FirstOrDefaultAsync(p => p.MaPhong == id);

                if (phong == null)
                {
                    TempData["Error"] = "Không tìm thấy phòng cần xóa.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Phongs.Remove(phong);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Đã xóa phòng có mã {MaPhong}", id);

                TempData["Success"] =
                    $"Đã xóa phòng {id} thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex, "Không thể xóa phòng {MaPhong}", id);

                TempData["Error"] =
                    "Không thể xóa phòng vì đang được dữ liệu khác tham chiếu. " +
                    "Hãy kiểm tra đặt phòng, hóa đơn hoặc lịch sử sử dụng.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Lỗi khi xóa phòng {MaPhong}", id);

                TempData["Error"] =
                    "Đã xảy ra lỗi khi xóa phòng. Vui lòng thử lại.";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            string id,
            string trangThai)
        {
            if (id == null)
                return NotFound();

            var phong = await _context.Phongs
                .FirstOrDefaultAsync(p => p.MaPhong == id);

            if (phong == null)
                return NotFound();

            var trangThaiHopLe = new[]
            {
                "Trống",
                "Đang sử dụng",
                "Bảo trì"
            };

            if (!trangThaiHopLe.Contains(trangThai))
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";

                return RedirectToAction(nameof(Index));
            }

            phong.TrangThai = trangThai;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void LoadLoaiPhong(string selectedValue = null)
        {
            ViewData["MaLoaiPhong"] = new SelectList(
                _context.LoaiPhongs,
                "MaLoaiPhong",
                "TenLoai",
                selectedValue
            );
        }
    }
}