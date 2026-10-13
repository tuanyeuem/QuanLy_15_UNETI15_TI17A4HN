// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Dữ liệu mẫu tối thiểu cho tài khoản, loại phòng, phòng, khách và giao dịch.
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Helpers;
using QuanLy_15_UNETI15_TI17A4HN.Models;
namespace QuanLy_15_UNETI15_TI17A4HN.Data;
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        var taiKhoanMau = new (string User,string Pass,string Name,string Email,string Role,bool Active)[]
        {
            ("admin1","Admin@123","Nguyễn Đình Tuấn","admin1@khachsan.vn",VaiTroTaiKhoan.Admin,true),
            ("admin2","Admin@123","Trần Quốc Bảo","admin2@khachsan.vn",VaiTroTaiKhoan.Admin,true),
            ("nhanvien1","NhanVien@123","Lê Thị Hoa","nv1@khachsan.vn",VaiTroTaiKhoan.NhanVien,true),
            ("nhanvien2","NhanVien@123","Phạm Văn Nam","nv2@khachsan.vn",VaiTroTaiKhoan.NhanVien,true),
            ("nhanvien3","NhanVien@123","Vũ Thị Lan","nv3@khachsan.vn",VaiTroTaiKhoan.NhanVien,true),
            ("khach1","KhachHang@123","Hoàng Minh Đức","khach1@gmail.com",VaiTroTaiKhoan.KhachHang,true),
            ("khach2","KhachHang@123","Đỗ Thu Trang","khach2@gmail.com",VaiTroTaiKhoan.KhachHang,true),
            ("khach3","KhachHang@123","Bùi Anh Tú","khach3@gmail.com",VaiTroTaiKhoan.KhachHang,true),
            ("khach4","KhachHang@123","Ngô Mai Linh","khach4@gmail.com",VaiTroTaiKhoan.KhachHang,true),
            ("khachkhoa","KhachHang@123","Tài khoản bị khóa","khoa@gmail.com",VaiTroTaiKhoan.KhachHang,false)
        };
        foreach(var a in taiKhoanMau)
            if(!db.TaiKhoans.Any(x=>x.TenDangNhap==a.User))
                db.TaiKhoans.Add(new TaiKhoan{TenDangNhap=a.User,MatKhau=MatKhauHelper.BamMatKhau(a.Pass),HoTen=a.Name,Email=a.Email,VaiTro=a.Role,TrangThai=a.Active});

        var soTaiKhoanKhach=db.TaiKhoans.Count(x=>x.VaiTro==VaiTroTaiKhoan.KhachHang);
        for(int i=1; soTaiKhoanKhach<30; i++)
        {
            var user=$"khach{i:00}";
            if(!db.TaiKhoans.Any(x=>x.TenDangNhap==user))
            {
                db.TaiKhoans.Add(new TaiKhoan{TenDangNhap=user,MatKhau=MatKhauHelper.BamMatKhau("KhachHang@123"),HoTen=$"Khách hàng {i:00}",Email=$"{user}@gmail.com",VaiTro=VaiTroTaiKhoan.KhachHang,TrangThai=true});
                soTaiKhoanKhach++;
            }
        }

        var loaiMau=new (string Name,int Capacity,decimal Price,string Description)[]
        {
            ("Standard Đơn",1,400000m,"Phòng một giường đơn."),
            ("Standard Đôi",2,600000m,"Phòng tiêu chuẩn giường đôi."),
            ("Deluxe",3,900000m,"Phòng rộng, có ban công."),
            ("Family",5,1400000m,"Phòng gia đình."),
            ("Suite",4,2000000m,"Phòng cao cấp, có phòng khách riêng.")
        };
        foreach(var l in loaiMau)
            if(!db.LoaiPhongs.Any(x=>x.TenLoai==l.Name))
                db.LoaiPhongs.Add(new LoaiPhong{TenLoai=l.Name,SoNguoiToiDa=l.Capacity,DonGiaNgay=l.Price,MoTa=l.Description,TrangThai=true});
        db.SaveChanges();

        var khachTaiKhoan=db.TaiKhoans.Where(x=>x.VaiTro==VaiTroTaiKhoan.KhachHang).OrderBy(x=>x.MaTaiKhoan).Take(30).ToList();
        for(int i=0;i<khachTaiKhoan.Count;i++)
        {
            var tk=khachTaiKhoan[i];
            if(!db.KhachHangs.Any(x=>x.MaTaiKhoan==tk.MaTaiKhoan))
                db.KhachHangs.Add(new KhachHang{MaTaiKhoan=tk.MaTaiKhoan,HoTen=tk.HoTen,Email=tk.Email,CCCD=$"079{i+1:000000000}",SoDienThoai=$"090{i+1:0000000}",NgaySinh=new DateTime(1980+i%20,1+i%12,1+i%27),GioiTinh=i%2==0?"Nam":"Nữ",DiaChi=$"Quận {(i%12)+1}, TP. Hồ Chí Minh",QuocTich="Việt Nam",TrangThai=tk.TrangThai});
        }
        db.SaveChanges();
        var khachs=db.KhachHangs.OrderBy(x=>x.MaKhachHang).Take(30).ToList();
        var loais=db.LoaiPhongs.OrderBy(x=>x.MaLoaiPhong).ToList();
        for(int i=1;i<=15;i++)
        {
            var soPhong=(100+i).ToString();
            if(!db.Phongs.Any(x=>x.SoPhong==soPhong))
            {
                var loai=loais[(i-1)%loais.Count];
                db.Phongs.Add(new Phong{SoPhong=soPhong,MaLoaiPhong=loai.MaLoaiPhong,Tang=(i-1)/5+1,HuongPhong=i%2==0?"Đông":"Tây",TienNghi="Wi-Fi, điều hòa, TV, minibar",DonGia=loai.DonGiaNgay,TrangThai=TrangThaiPhong.Trong});
            }
        }
        var dichVuMau=new (string Name,string Unit,decimal Price)[]
        {
            ("Bữa sáng","suất",120000m),("Giặt ủi","kg",50000m),("Đưa đón sân bay","chuyến",300000m),("Nước uống","chai",20000m),
            ("Spa","lượt",250000m),("Thuê xe","ngày",400000m),("Minibar","lần",100000m),("Ăn tại phòng","suất",180000m)
        };
        foreach(var d in dichVuMau)
            if(!db.DichVus.Any(x=>x.TenDichVu==d.Name))
                db.DichVus.Add(new DichVu{TenDichVu=d.Name,DonViTinh=d.Unit,DonGia=d.Price,TrangThai=true});
        db.SaveChanges();

        var rooms=db.Phongs.Include(x=>x.LoaiPhong).OrderBy(x=>x.MaPhong).ToList();
        var staff=db.TaiKhoans.Where(x=>x.VaiTro==VaiTroTaiKhoan.NhanVien).OrderBy(x=>x.MaTaiKhoan).ToList();
        var today=DateTime.UtcNow.Date;
        for(int i=1;i<=45;i++)
        {
            var tag=$"[DEMO-DAT-{i:000}]";
            if(db.DatPhongs.Any(x=>x.GhiChu!=null&&x.GhiChu.Contains(tag))) continue;
            var room=rooms[(i-1)%rooms.Count];
            DateTime nhan=i<=15?today.AddDays(-45+i*2):i<=18?today.AddDays(-1):i<=33?today.AddDays(3+(i-19)*2):today.AddDays(-i);
            var state=i<=15?TrangThaiDatPhong.HoanThanh:i<=18?TrangThaiDatPhong.DangXuLy:i<=33?TrangThaiDatPhong.ChoXuLy:i<=40?TrangThaiDatPhong.Huy:TrangThaiDatPhong.TuChoi;
            db.DatPhongs.Add(new DatPhong{MaKhachHang=khachs[(i-1)%khachs.Count].MaKhachHang,MaNhanVienTiepNhan=state==TrangThaiDatPhong.ChoXuLy?null:staff[(i-1)%staff.Count].MaTaiKhoan,NgayDat=today.AddDays(-1-i%10),NgayNhan=nhan,NgayTraDuKien=nhan.AddDays(2),SoNguoi=Math.Min(room.LoaiPhong!.SoNguoiToiDa,1+i%3),TienCoc=state==TrangThaiDatPhong.HoanThanh||state==TrangThaiDatPhong.DangXuLy?room.DonGia*0.2m:0,TrangThai=state,GhiChu=$"{tag} Dữ liệu mẫu kiểm thử."});
        }
        db.SaveChanges();

        var sampleBookings=db.DatPhongs.Where(x=>x.GhiChu!=null&&x.GhiChu.Contains("[DEMO-DAT-")).OrderBy(x=>x.MaDatPhong).ToList();
        var services=db.DichVus.OrderBy(x=>x.MaDichVu).ToList();
        for(int i=1;i<=18;i++)
        {
            var tag=$"[DEMO-DAT-{i:000}]";
            var booking=sampleBookings.FirstOrDefault(x=>x.GhiChu!.Contains(tag));
            if(booking==null) continue;
            var room=rooms[(i-1)%rooms.Count];
            if(!db.ChiTietDatPhongs.Any(x=>x.MaDatPhong==booking.MaDatPhong&&x.MaPhong==room.MaPhong))
                db.ChiTietDatPhongs.Add(new ChiTietDatPhong{MaDatPhong=booking.MaDatPhong,MaPhong=room.MaPhong,NgayNhan=booking.NgayNhan,NgayTra=booking.NgayTraDuKien,DonGia=room.DonGia,SoNgay=2,ThanhTien=room.DonGia*2,TrangThai=i<=15?TrangThaiChiTietDatPhong.DaTraPhong:TrangThaiChiTietDatPhong.DangSuDung});
            if(i>15) room.TrangThai=TrangThaiPhong.DangSuDung;
        }
        db.SaveChanges();
        var details=db.ChiTietDatPhongs.OrderBy(x=>x.MaChiTiet).ToList();
        for(int i=db.SuDungDichVus.Count();i<20&&details.Count>0;i++)
        {
            var d=services[i%services.Count];
            var qty=1+i%3;
            db.SuDungDichVus.Add(new SuDungDichVu{MaChiTiet=details[i%details.Count].MaChiTiet,MaDichVu=d.MaDichVu,SoLuong=qty,DonGia=d.DonGia,ThanhTien=d.DonGia*qty,NgaySuDung=details[i%details.Count].NgayNhan.AddDays(1)});
        }
        db.SaveChanges();
    }
}
