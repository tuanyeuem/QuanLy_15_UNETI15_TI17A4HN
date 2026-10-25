// Họ và tên: Vũ Kiều Trinh
// Mã sinh viên: 23103100199
// Nội dung thực hiện: Module 2 - Quản lý phòng, CRUD, trạng thái, tìm kiếm/lọc/sắp xếp/phân trang.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
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


        [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
        public async Task<IActionResult> Index(int trang = 1)
        {
            const int soDongMoiTrang = 5;

            // Đếm tổng số phòng
            int tongSoPhong = await _context.Phongs.CountAsync();

            // Tính tổng số trang
            int tongSoTrang = Math.Max(
                1,
                (int)Math.Ceiling(tongSoPhong / (double)soDongMoiTrang));

            // Đảm bảo trang hợp lệ
            trang = Math.Clamp(trang, 1, tongSoTrang);

            // Lưu thông tin phân trang
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
            ViewBag.TongSoPhong = tongSoPhong;

            // Chỉ lấy phòng thuộc trang hiện tại
            var dsPhong = await _context.Phongs
                .AsNoTracking()
                .Include(p => p.LoaiPhong)
                .OrderBy(p => p.Tang)
                .ThenBy(p => p.SoPhong)
                .Skip((trang - 1) * soDongMoiTrang)
                .Take(soDongMoiTrang)
                .ToListAsync();

            return View(dsPhong);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var phong = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .FirstOrDefaultAsync(p => p.MaPhong == id);

            if (phong == null)
                return NotFound();

            return View(phong);
        }
        [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
        public IActionResult Create()
        {
            LoadLoaiPhong();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
        public async Task<IActionResult> Create(Phong phong)
        {
            _logger.LogInformation(
                "Bắt đầu thêm phòng. MaPhong={MaPhong}, SoPhong={SoPhong}, MaLoaiPhong={MaLoaiPhong}",
                phong.MaPhong, phong.SoPhong, phong.MaLoaiPhong);

            try
            {
                phong.SoPhong = (phong.SoPhong ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(phong.SoPhong))
                {
                    ModelState.AddModelError(
                        nameof(phong.SoPhong), "Số phòng không được để trống.");
                }

                if (phong.MaLoaiPhong <= 0)
                {
                    ModelState.AddModelError(
                        nameof(phong.MaLoaiPhong), "Vui lòng chọn loại phòng.");
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

                if (phong.MaLoaiPhong > 0)
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
        [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
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
        [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
        public async Task<IActionResult> Edit(int id, Phong phong)
        {
            if (id <= 0 || id != phong.MaPhong)
            {
                _logger.LogWarning(
                    "Edit POST: Mã phòng không hợp lệ. Id={Id}, MaPhong={MaPhong}",
                    id, phong.MaPhong);

                return NotFound();
            }

            try
            {
                phong.SoPhong = phong.SoPhong?.Trim()!;

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

                if (phong.MaLoaiPhong > 0)
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

        [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
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
        [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
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
        [PhanQuyen(VaiTroTaiKhoan.Admin, VaiTroTaiKhoan.NhanVien)]
        public async Task<IActionResult> CapNhatTrangThai(
            int id,
            string trangThai)
        {
            if (id <= 0)
                return NotFound();

            var phong = await _context.Phongs
                .FirstOrDefaultAsync(p => p.MaPhong == id);

            if (phong == null)
                return NotFound();

            // Chấp nhận cả mã trạng thái và nhãn tiếng Việt cũ để không làm hỏng dữ liệu.
            var bangTrangThai = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["TrongRanh"] = TrangThaiPhong.TrongRanh,
                ["Trống"] = TrangThaiPhong.TrongRanh,
                ["Còn trống"] = TrangThaiPhong.TrongRanh,
                ["DangSuDung"] = TrangThaiPhong.DangSuDung,
                ["Đang sử dụng"] = TrangThaiPhong.DangSuDung,
                ["BaoTri"] = TrangThaiPhong.BaoTri,
                ["Bảo trì"] = TrangThaiPhong.BaoTri,
                ["KhongKinhDoanh"] = TrangThaiPhong.KhongKinhDoanh,
                ["Ngừng kinh doanh"] = TrangThaiPhong.NgungKinhDoanh,
                ["NgungKinhDoanh"] = TrangThaiPhong.NgungKinhDoanh
            };

            if (!bangTrangThai.TryGetValue(trangThai ?? string.Empty, out var maTrangThai))
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";

                return RedirectToAction(nameof(Index));
            }

            phong.TrangThai = maTrangThai;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã cập nhật trạng thái phòng {phong.SoPhong}.";

            return RedirectToAction(nameof(Index));
        }

        private void LoadLoaiPhong(int? selectedValue = null)
        {
            ViewData["MaLoaiPhong"] = new SelectList(
                _context.LoaiPhongs,
                "MaLoaiPhong",
                "TenLoai",
                selectedValue
            );
        }


        public async Task<IActionResult> TimKiem(
            string? tuKhoa,
            string? maLoaiPhong,
            int? tang,
            string? trangThai,
            decimal? giaTu,
            decimal? giaDen,
            DateTime? ngayNhan,
            DateTime? ngayTra,
            string? sapXep,
            int trang = 1)
        {
            IQueryable<Phong> query = _context.Phongs
                .AsNoTracking()
                .Include(p => p.LoaiPhong);

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                query = query.Where(p =>
                    p.SoPhong.Contains(tuKhoa) ||
                    (p.LoaiPhong != null && p.LoaiPhong.TenLoai.Contains(tuKhoa)) ||
                    _context.DatPhongs.Any(d =>
                        d.MaPhong == p.MaPhong && d.KhachHang.HoTen.Contains(tuKhoa)) ||
                    _context.ChiTietDatPhongs.Any(c =>
                        c.MaPhong == p.MaPhong && c.DatPhong.KhachHang.HoTen.Contains(tuKhoa)));
            }

            if (int.TryParse(maLoaiPhong, out var maLoaiPhongId) && maLoaiPhongId > 0)
            {
                query = query.Where(p => p.MaLoaiPhong == maLoaiPhongId);
            }

            if (tang.HasValue)
            {
                query = query.Where(p => p.Tang == tang.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThai) &&
                trangThai != "all")
            {
                query = query.Where(p => p.TrangThai == trangThai);
            }

            if (giaTu.HasValue)
            {
                query = query.Where(p => p.DonGia >= giaTu.Value);
            }

            if (giaDen.HasValue)
            {
                query = query.Where(p => p.DonGia <= giaDen.Value);
            }

            // Lọc theo khoảng ngày còn trống. Nếu chỉ nhập một đầu ngày, tự suy ra đầu còn lại
            // để bộ lọc vẫn có ý nghĩa thay vì bị bỏ qua.
            if (ngayNhan.HasValue || ngayTra.HasValue)
            {
                if (!ngayNhan.HasValue)
                    ngayNhan = DateTime.Today;
                if (!ngayTra.HasValue)
                    ngayTra = ngayNhan.Value.AddDays(1);

                var nhanLoc = ngayNhan.Value.Date;
                var traLoc = ngayTra.Value.Date;

                if (traLoc <= nhanLoc)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Ngày trả phòng phải sau ngày nhận phòng.");
                }
                else
                {
                    // Chỉ hiển thị phòng đang kinh doanh khi tìm theo ngày.
                    if (string.IsNullOrWhiteSpace(trangThai) || trangThai == "all")
                    {
                        query = query.Where(p =>
                            p.TrangThai != TrangThaiPhong.BaoTri &&
                            p.TrangThai != TrangThaiPhong.KhongKinhDoanh &&
                            p.TrangThai != TrangThaiPhong.NgungKinhDoanh);
                    }

                    query = query.Where(p =>
                        !_context.DatPhongs.Any(d =>
                            d.MaPhong == p.MaPhong &&
                            d.TrangThai != TrangThaiDatPhong.Huy &&
                            d.TrangThai != TrangThaiDatPhong.TuChoi &&
                            d.NgayNhan < traLoc &&
                            d.NgayTraDuKien > nhanLoc) &&
                        !_context.ChiTietDatPhongs.Any(c =>
                            c.MaPhong == p.MaPhong &&
                            c.TrangThai != TrangThaiChiTiet.DaHuy &&
                            c.NgayNhan < traLoc &&
                            c.NgayTra > nhanLoc));
                }
            }

            ViewBag.LoaiPhongs = new SelectList(
                await _context.LoaiPhongs
                    .AsNoTracking()
                    .OrderBy(l => l.TenLoai)
                    .ToListAsync(),
                "MaLoaiPhong",
                "TenLoai",
                maLoaiPhong);

            IOrderedQueryable<Phong> danhSachSapXep;

            switch (sapXep)
            {
                case "soPhongTang":
                    danhSachSapXep = query
                        .OrderBy(p => p.SoPhong);
                    break;

                case "soPhongGiam":
                    danhSachSapXep = query
                        .OrderByDescending(p => p.SoPhong);
                    break;

                case "donGiaTang":
                    danhSachSapXep = query
                        .OrderBy(p => p.DonGia);
                    break;

                case "donGiaGiam":
                    danhSachSapXep = query
                        .OrderByDescending(p => p.DonGia);
                    break;

                case "tangTang":
                    danhSachSapXep = query
                        .OrderBy(p => p.Tang)
                        .ThenBy(p => p.SoPhong);
                    break;

                case "tangGiam":
                    danhSachSapXep = query
                        .OrderByDescending(p => p.Tang)
                        .ThenBy(p => p.SoPhong);
                    break;

                case "tenLoaiTang":
                    danhSachSapXep = query
                        .OrderBy(
                            p => p.LoaiPhong != null
                                ? p.LoaiPhong.TenLoai
                                : "");
                    break;

                default:
                    danhSachSapXep = query
                        .OrderBy(p => p.Tang)
                        .ThenBy(p => p.SoPhong);
                    break;
            }


            ViewBag.TuKhoa = tuKhoa;
            ViewBag.MaLoaiPhong = maLoaiPhong ?? "all";
            ViewBag.Tang = tang;
            ViewBag.TrangThai = trangThai ?? "all";
            ViewBag.GiaTu = giaTu;
            ViewBag.GiaDen = giaDen;
            ViewBag.NgayNhan = ngayNhan?.ToString("yyyy-MM-dd");
            ViewBag.NgayTra = ngayTra?.ToString("yyyy-MM-dd");
            ViewBag.SapXep = sapXep ?? "soPhongTang";

            const int soDongMoiTrang = 5;

            int tongSoPhong = await query.CountAsync();

            int tongSoTrang = Math.Max(
                1,
                (int)Math.Ceiling(
                    tongSoPhong / (double)soDongMoiTrang));

            trang = Math.Clamp(trang, 1, tongSoTrang);

            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
            ViewBag.TongSoPhong = tongSoPhong;
            ViewBag.SoDongMoiTrang = soDongMoiTrang;

            var danhSachPhong = await danhSachSapXep
                .Skip((trang - 1) * soDongMoiTrang)
                .Take(soDongMoiTrang)
                .ToListAsync();

            return View("TimKiem", danhSachPhong);

        }
    }
}