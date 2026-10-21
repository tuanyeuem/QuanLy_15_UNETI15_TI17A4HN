# Quản lý khách sạn – QuanLy_15_UNETI15_TI17A4HN

Ứng dụng web **quản lý khách sạn** xây dựng bằng **ASP.NET Core MVC (.NET 10)** và **Entity Framework Core + SQL Server**. Hệ thống hỗ trợ quản lý phòng, đặt phòng, chi tiết đặt phòng (nhiều phòng / một lần đặt), tính tiền và thống kê doanh thu, có phân quyền theo vai trò.

## Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Framework | ASP.NET Core MVC, `net10.0` |
| ORM | Entity Framework Core 10 (Code First + Migrations) |
| Cơ sở dữ liệu | SQL Server (mặc định: LocalDB) |
| Giao diện | Razor Views, Bootstrap, jQuery Validation |
| Xác thực | Session (hết hạn sau 30 phút không hoạt động), mật khẩu băm SHA-256 |

## Chức năng chính

- **Tài khoản & phân quyền**: đăng nhập / đăng xuất; 3 vai trò `Admin`, `NhanVien`, `KhachHang` kiểm soát bằng attribute `[PhanQuyen]`; trang "Không có quyền" khi truy cập sai vai trò.
- **Quản lý phòng**: xem danh sách có phân trang, xem chi tiết, thêm / sửa / xóa, cập nhật trạng thái (Sẵn sàng, Đang sử dụng, Bảo trì, Không kinh doanh), tìm kiếm phòng.
- **Quản lý đặt phòng** (Admin, Nhân viên): lọc danh sách, xem chi tiết, **duyệt** → **nhận phòng** → **trả phòng**, hoặc **hủy** kèm lý do.
- **Chi tiết đặt phòng**: thêm / sửa / xóa từng phòng trong một lượt đặt, tự tính số ngày và thành tiền, **chốt kết quả** khi trả phòng.
- **Khách hàng**: xem danh sách đặt phòng của chính mình ("Đặt phòng của tôi").
- **Thống kê**: Dashboard tổng quan, thống kê theo năm, báo cáo theo khoảng ngày và **xuất CSV**.

## Phân quyền theo vai trò

| Chức năng | Admin | Nhân viên | Khách hàng |
|---|:---:|:---:|:---:|
| Đặt phòng (duyệt / nhận / trả / hủy) | ✅ | ✅ | ❌ |
| Chi tiết đặt phòng (thêm / sửa / xóa / chốt) | ✅ | ✅ | ❌ |
| Thống kê, Dashboard, xuất CSV | ✅ | ✅ | ❌ |
| Xem kết quả của một lượt đặt phòng | ✅ | ✅ | ✅ |
| Đặt phòng của tôi | ❌ | ❌ | ✅ |

## Quy trình trạng thái đặt phòng

```
ChoDuyet ──► DaDuyet ──► DaNhanPhong ──► DaTraPhong
    │            │
    └────────────┴──────────► Huy
```

Ngoài ra, luồng đặt nhiều phòng dùng các trạng thái tổng hợp: `ChoXuLy`, `DangXuLy`, `HoanThanh`, `DaHuy`.

**Quy tắc tính tiền**: `Thành tiền = Đơn giá × Số ngày`, trong đó số ngày tính theo đêm và **tối thiểu là 1 ngày** (nhận và trả cùng ngày vẫn tính 1 ngày).

## Cấu trúc thư mục

```
QuanLy_15_UNETI15_TI17A4HN/
├── QuanLy_15_UNETI15_TI17A4HN.slnx
├── README.md
└── QuanLy_15_UNETI15_TI17A4HN/
    ├── Controllers/     # Home, TaiKhoan, Phong, DatPhong, ChiTietDatPhong, ThongKe
    ├── Models/          # TaiKhoan, KhachHang, LoaiPhong, Phong, DatPhong, ChiTietDatPhong, DichVu
    ├── ViewModels/      # DangNhap, DatPhongList, ChiTietDatPhong, ThongKe
    ├── Views/           # Razor views theo từng controller + Shared
    ├── Data/            # AppDbContext, DbSeeder, DuLieuMauGiaoDich (dữ liệu mẫu)
    ├── Helpers/         # PhanQuyen, MatKhauHelper, TinhTienHelper, DanhSachPhanTrang, TrangThaiUi
    ├── Migrations/      # Các migration của EF Core
    ├── wwwroot/         # CSS, JS, thư viện front-end
    ├── appsettings.json # Chuỗi kết nối CSDL
    └── Program.cs       # Cấu hình DI, Session, routing, tự động migrate + seed
```

## Cơ sở dữ liệu

Các bảng: `TaiKhoans`, `KhachHangs`, `LoaiPhongs`, `Phongs`, `DatPhongs`, `ChiTietDatPhongs`, `DichVus`.

```
TaiKhoan 1──0..1 KhachHang 1──* DatPhong *──1 Phong *──1 LoaiPhong
                                   │
                                   └──* ChiTietDatPhong *──1 Phong
```

Chuỗi kết nối mặc định trong `appsettings.json`:

```
Server=(localdb)\MSSQLLocalDB;Database=QuanLyKhachSan;Trusted_Connection=True;TrustServerCertificate=True
```

Nếu dùng SQL Server khác, hãy sửa `DefaultConnection` cho phù hợp.

## Hướng dẫn chạy

**Yêu cầu:** .NET SDK 10, SQL Server hoặc SQL Server LocalDB, Visual Studio 2022+ hoặc VS Code.

```bash
git clone https://github.com/tuanyeuem/QuanLy_15_UNETI15_TI17A4HN.git
cd QuanLy_15_UNETI15_TI17A4HN/QuanLy_15_UNETI15_TI17A4HN
dotnet restore
dotnet run --launch-profile https
```

Khi khởi động, ứng dụng **tự động áp dụng migration và nạp dữ liệu mẫu**, không cần chạy `dotnet ef database update` thủ công.

Địa chỉ truy cập: `https://localhost:7110` hoặc `http://localhost:5058`.

## Tài khoản dùng thử

Dữ liệu mẫu được tạo ở lần chạy đầu tiên (dùng để học tập / kiểm thử, **không dùng cho môi trường thật**).

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Admin | `admin1`, `admin2` | `Admin@123` |
| Nhân viên | `nhanvien1`, `nhanvien2`, `nhanvien3` | `NhanVien@123` |
| Khách hàng | `khach1` … `khach4` | `KhachHang@123` |
| Tài khoản bị khóa | `khachkhoa` | `KhachHang@123` |

Loại phòng mẫu: Standard Đơn, Standard Đôi, Deluxe, Family, Suite.

## Thành viên thực hiện

| Họ và tên | Mã sinh viên | Phần phụ trách |
|---|---|---|
| Lê Văn Thanh | 23103100200 | Module 5: tài khoản & phân quyền, chi tiết đặt phòng, thống kê, tính tiền, DbContext |
| Vũ Xuân Tuấn | 23103100035 | Module 4: quản lý đặt phòng (duyệt / nhận / trả / hủy) |

Lớp: TI17A4HN – UNETI.

