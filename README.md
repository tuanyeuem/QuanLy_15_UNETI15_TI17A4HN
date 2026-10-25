# Quản lý khách sạn – QuanLy_15_UNETI15_TI17A4HN

Ứng dụng web quản lý khách sạn xây dựng bằng **ASP.NET Core MVC (.NET 10)**, **Entity Framework Core 10** và **SQL Server (LocalDB)**. Hệ thống hỗ trợ quản lý phòng, loại phòng, khách hàng, đặt nhiều phòng trong một lượt, duyệt – nhận – trả – hủy đặt phòng, tính tiền và thống kê doanh thu; phân quyền theo vai trò `Admin`, `NhanVien`, `KhachHang`.

## Thành viên nhóm

| Họ và tên | MSSV | Phần việc chính |
|---|---|---|
| Nguyễn Đình Tuấn | 23103100235 | Module 1: đăng nhập, đăng xuất, Session, phân quyền, quản lý tài khoản; hỗ trợ quản lý loại phòng |
| Thành viên Module 2 (Git: `tkv261005`) | *(bổ sung MSSV khi nộp)* | Module 2: quản lý phòng, loại phòng, ảnh phòng, tìm kiếm – lọc – sắp xếp – phân trang |
| Nguyễn Đình Quang | 23103100259 | Module 3: khách hàng, hồ sơ khách hàng, đặt phòng của khách |
| Vũ Xuân Tuấn | 23103100035 | Module 4: duyệt, nhận phòng, trả phòng, hủy đặt phòng (hỗ trợ nhiều phòng/lượt) |
| Lê Văn Thanh | 23103100200 | Module 5: chi tiết đặt phòng, tính tiền, dashboard, thống kê, báo cáo, dữ liệu mẫu |

> Ghi chú: repository không lưu họ tên/MSSV của tài khoản Git `tkv261005`, nên dòng Module 2 đang để trống MSSV. Nhóm điền trực tiếp trước khi nộp nếu nhà trường yêu cầu.

## Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Framework | ASP.NET Core MVC, `net10.0` |
| ORM | Entity Framework Core 10 (Code First + Migrations) |
| Cơ sở dữ liệu | SQL Server / LocalDB |
| Giao diện | Razor Views, Bootstrap, jQuery Validation |
| Xác thực | Session (hết hạn sau 30 phút), mật khẩu băm SHA-256 |

## Chức năng chính

- **Tài khoản & phân quyền:** đăng nhập/đăng xuất; kiểm tra vai trò bằng `[PhanQuyen]`; khóa/mở khóa tài khoản; đặt lại mật khẩu.
- **Quản lý phòng:** CRUD phòng và loại phòng; trạng thái `TrongRanh`, `DangSuDung`, `BaoTri`, `KhongKinhDoanh`, `NgungKinhDoanh`; tải lên/xóa ảnh phòng.
- **Tìm kiếm phòng:** theo số phòng, loại phòng hoặc tên khách hàng; lọc theo tầng, trạng thái, khoảng giá và **khoảng ngày còn trống**; sắp xếp và phân trang.
- **Đặt phòng:** khách hàng chọn phòng và ngày; hệ thống kiểm tra trùng lịch theo cả phòng chính và chi tiết phòng; hỗ trợ đặt nhiều phòng trong một lượt.
- **Nghiệp vụ đặt phòng:** `ChoDuyet → DaDuyet → DaNhanPhong → DaTraPhong`; có nhánh `Huy`/`TuChoi`; kiểm tra sức chứa, trạng thái phòng và trùng lịch trước khi duyệt/nhận phòng.
- **Tính tiền & thống kê:** tính số ngày, thành tiền theo đơn giá; dashboard doanh thu, thống kê theo tháng/loại phòng/trạng thái.

## Cấu trúc thư mục chính

```text
QuanLy_15_UNETI15_TI17A4HN/
├── Controllers/        # TaiKhoan, Phong, LoaiPhong, KhachHang, DatPhong, ChiTietDatPhong, ThongKe...
├── Data/               # AppDbContext, DbSeeder, DuLieuMauGiaoDich
├── Helpers/            # PhanQuyenAttribute, MatKhauHelper, TinhTienHelper, PhongAnhHelper...
├── Migrations/         # Lịch sử migration EF Core
├── Models/             # TaiKhoan, Phong, LoaiPhong, KhachHang, DatPhong, ChiTietDatPhong...
├── ViewModels/         # ViewModel cho đặt phòng, tài khoản, thống kê...
├── Views/              # Razor Views theo controller
└── wwwroot/            # CSS, JS, ảnh phòng mặc định, ảnh tải lên
```

## Yêu cầu môi trường

- .NET SDK 10
- SQL Server LocalDB (mặc định) hoặc SQL Server Express/Developer
- Visual Studio 2022/2026 hoặc VS Code + C# Dev Kit

## Cách chạy

1. Mở terminal tại thư mục chứa file `.csproj`:
   ```powershell
   cd QuanLy_15_UNETI15_TI17A4HN
   ```
2. Kiểm tra chuỗi kết nối trong `appsettings.json`:
   ```json
   "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=QuanLyKhachSan;Trusted_Connection=True;TrustServerCertificate=True"
   ```
3. Khôi phục và chạy:
   ```powershell
   dotnet restore
   dotnet run
   ```
4. Mở trình duyệt tại địa chỉ hiển thị trong terminal (thường là `https://localhost:xxxx`).

Khi khởi động, ứng dụng tự động:
- chạy migration (`Database.Migrate()`);
- nạp tài khoản, loại phòng, phòng, dịch vụ mẫu (`DbSeeder`);
- bổ sung dữ liệu giao dịch mẫu idempotent (`DuLieuMauGiaoDich`).

## Tài khoản thử nghiệm

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Admin | `admin1` | `Admin@123` |
| Admin | `admin2` | `Admin@123` |
| Nhân viên | `nhanvien1` | `NhanVien@123` |
| Nhân viên | `nhanvien2` | `NhanVien@123` |
| Nhân viên | `nhanvien3` | `NhanVien@123` |
| Khách hàng | `khach1` | `KhachHang@123` |
| Khách hàng | `khach2` | `KhachHang@123` |
| Khách hàng | `khach3` | `KhachHang@123` |
| Khách hàng | `khach4` | `KhachHang@123` |
| Khách hàng bị khóa | `khachkhoa` | `KhachHang@123` |

## Dữ liệu mẫu

Sau lần chạy đầu tiên trên cơ sở dữ liệu trống, hệ thống có khoảng:

- 10 tài khoản (9 đang hoạt động, 1 khách bị khóa);
- 5 loại phòng, 15 phòng, 5 dịch vụ;
- **30 khách hàng**;
- **45 đặt phòng** với đủ trạng thái: chờ duyệt, đã duyệt, đang ở, đã trả, đã hủy, bị từ chối;
- **18 chi tiết đặt phòng** gắn với các lượt đã trả/đang ở để kiểm thử tính tiền và thống kê.

Bộ nạp dữ liệu dùng cờ `[SEED-KH]` / `[SEED-BK]` và kiểm tra số lượng trước khi thêm, nên chạy lại không nhân đôi dữ liệu.

## Ghi chú nghiệp vụ

- Ngày trả phải sau ngày nhận; hệ thống tính tối thiểu 1 ngày.
- Khi tìm phòng theo ngày, chỉ hiển thị phòng đang kinh doanh và không trùng lịch với đặt phòng/chi tiết đặt phòng còn hiệu lực.
- Khi duyệt hoặc nhận phòng, hệ thống kiểm tra **tất cả phòng trong chi tiết**, không chỉ phòng chính; phòng bảo trì/ngừng kinh doanh bị chặn.
- Hủy đặt phòng chỉ áp dụng cho lượt `ChoDuyet`/`DaDuyet`; chi tiết chưa nhận được chuyển sang `DaHuy`.
- Ảnh phòng tải lên được lưu tại `wwwroot/uploads/phong/`; nếu chưa có ảnh riêng, dùng ảnh SVG mặc định trong `wwwroot/images/phong/`.
