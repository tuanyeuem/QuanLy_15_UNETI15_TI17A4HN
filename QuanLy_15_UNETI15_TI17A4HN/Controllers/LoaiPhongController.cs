// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Admin CRUD loại phòng, kiểm tra trùng và tham chiếu khi xóa.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
namespace QuanLy_15_UNETI15_TI17A4HN.Controllers;
[PhanQuyen(VaiTroTaiKhoan.Admin)]
public class LoaiPhongController : Controller {
 private readonly AppDbContext _context;
 public LoaiPhongController(AppDbContext context)=>_context=context;
 public async Task<IActionResult> Index(string? tuKhoa,string? trangThai) {
  var q=_context.LoaiPhongs.AsNoTracking().AsQueryable();
  if(!string.IsNullOrWhiteSpace(tuKhoa)){var k=tuKhoa.Trim();q=q.Where(x=>x.TenLoai.Contains(k));}
  if(trangThai=="hoatdong")q=q.Where(x=>x.TrangThai);
  if(trangThai=="ngung")q=q.Where(x=>!x.TrangThai);
  ViewData["TuKhoa"]=tuKhoa;ViewData["TrangThai"]=trangThai;
  return View(await q.OrderBy(x=>x.TenLoai).ToListAsync());
 }
 public async Task<IActionResult> Details(int? id){if(id==null)return NotFound();var x=await _context.LoaiPhongs.AsNoTracking().FirstOrDefaultAsync(a=>a.MaLoaiPhong==id);return x==null?NotFound():View(x);}
 [HttpGet] public IActionResult Create()=>View(new LoaiPhong());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(LoaiPhong model){
  model.TenLoai=model.TenLoai?.Trim()??string.Empty;await CheckName(model.TenLoai);
  if(!ModelState.IsValid)return View(model);_context.LoaiPhongs.Add(model);
  try{await _context.SaveChangesAsync();TempData["ThongBao"]=$"Đã thêm loại phòng “{model.TenLoai}”.";return RedirectToAction(nameof(Index));}
  catch(DbUpdateException){ModelState.AddModelError(nameof(model.TenLoai),"Tên loại phòng đã tồn tại.");return View(model);}
 }
 [HttpGet] public async Task<IActionResult> Edit(int? id){if(id==null)return NotFound();var x=await _context.LoaiPhongs.FindAsync(id);return x==null?NotFound():View(x);}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id,LoaiPhong model){
  if(id!=model.MaLoaiPhong)return NotFound();model.TenLoai=model.TenLoai?.Trim()??string.Empty;await CheckName(model.TenLoai,id);
  if(!ModelState.IsValid)return View(model);var x=await _context.LoaiPhongs.FindAsync(id);if(x==null)return NotFound();
  x.TenLoai=model.TenLoai;x.SoNguoiToiDa=model.SoNguoiToiDa;x.DonGiaNgay=model.DonGiaNgay;x.MoTa=model.MoTa?.Trim();x.TrangThai=model.TrangThai;
  try{await _context.SaveChangesAsync();TempData["ThongBao"]=$"Đã cập nhật loại phòng “{x.TenLoai}”.";return RedirectToAction(nameof(Index));}
  catch(DbUpdateException){ModelState.AddModelError(nameof(model.TenLoai),"Tên loại phòng đã tồn tại.");return View(model);}
 }
 [HttpGet] public async Task<IActionResult> Delete(int? id){if(id==null)return NotFound();var x=await _context.LoaiPhongs.AsNoTracking().FirstOrDefaultAsync(a=>a.MaLoaiPhong==id);return x==null?NotFound():View(x);}
 [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken] public async Task<IActionResult> DeleteConfirmed(int id){
  var x=await _context.LoaiPhongs.FindAsync(id);if(x==null)return NotFound();_context.LoaiPhongs.Remove(x);
  try{await _context.SaveChangesAsync();TempData["ThongBao"]=$"Đã xóa loại phòng “{x.TenLoai}”.";}
  catch(DbUpdateException){TempData["Loi"]="Không thể xóa loại phòng đã được dữ liệu khác tham chiếu. Hãy chuyển trạng thái sang Ngừng kinh doanh.";}
  return RedirectToAction(nameof(Index));
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> DoiTrangThai(int id){var x=await _context.LoaiPhongs.FindAsync(id);if(x==null)return NotFound();x.TrangThai=!x.TrangThai;await _context.SaveChangesAsync();TempData["ThongBao"]=x.TrangThai?"Đã mở kinh doanh loại phòng.":"Đã ngừng kinh doanh loại phòng.";return RedirectToAction(nameof(Index));}
 private async Task CheckName(string name,int? except=null){if(!string.IsNullOrWhiteSpace(name)&&await _context.LoaiPhongs.AnyAsync(x=>x.TenLoai==name&&x.MaLoaiPhong!=except))ModelState.AddModelError(nameof(LoaiPhong.TenLoai),"Tên loại phòng đã tồn tại.");}
}