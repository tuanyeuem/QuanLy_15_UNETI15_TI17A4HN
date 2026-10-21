# Quản lý khách sạn – Module 5: Chi tiết đặt phòng, Tính tiền, Dashboard, Thống kê, Báo cáo

Nhánh `md5` của đồ án **QuanLy_15_UNETI15_TI17A4HN** (ASP.NET Core MVC .NET 10, EF Core, SQL Server). Nhánh này chỉ chứa phần việc của Module 5; các module khác (Phòng, Đặt phòng, Loại phòng, Quản lý tài khoản) nằm ở các nhánh riêng và được gộp tại `main`.

## Chức năng của module

| Nhóm | Mô tả | Quyền |
|---|---|---|
| Chi tiết đặt phòng | Danh sách có lọc / phân trang, xem chi tiết, thêm, sửa, xóa từng phòng trong một lượt đặt | Admin, Nhân viên |
| Tính tiền phòng | Tự tính số ngày ở và thành tiền (`TinhTienHelper`); **chốt kết quả** khi trả phòng | Admin, Nhân viên |
| Kết quả đặt phòng | Xem kết quả một lượt đặt; khách xem lại các đặt phòng của mình ("Đặt phòng của tôi") | Mọi người dùng đã đăng nhập / Khách hàng |
| Dashboard | Số liệu tổng quan: phòng, đặt phòng, doanh thu | Admin, Nhân viên |
| Thống kê | Thống kê theo năm | Admin, Nhân viên |
| Báo cáo | Báo cáo theo khoảng ngày và **xuất CSV** | Admin, Nhân viên |

**Quy tắc tính tiền:** `Thành tiền = Đơn giá × Số ngày`. Số ngày tính theo đêm, **tối thiểu 1 ngày** (nhận và trả cùng ngày vẫn tính 1 ngày).

## Các file thuộc module

```
Controllers/  ChiTietDatPhongController.cs, ThongKeController.cs
ViewModels/   ChiTietDatPhongViewModels.cs, ThongKeViewModels.cs
Views/        ChiTietDatPhong/*, ThongKe/{Dashboard,Index,BaoCao}.cshtml
Helpers/      TinhTienHelper.cs, TrangThaiUi.cs, DanhSachPhanTrang.cs
Data/         DuLieuMauGiaoDich.cs (dữ liệu mẫu để thử Dashboard / Thống kê)
Models/       ChiTietDatPhong.cs
```

Phần dùng chung (đăng nhập, phân quyền `[PhanQuyen]`, các Model, `AppDbContext`, `DbSeeder`, Migrations) được giữ lại để project chạy độc lập.

## Cách chạy

Yêu cầu: .NET SDK 10, SQL Server hoặc LocalDB.

```bash
git clone -b md5 https://github.com/tuanyeuem/QuanLy_15_UNETI15_TI17A4HN.git
cd QuanLy_15_UNETI15_TI17A4HN/QuanLy_15_UNETI15_TI17A4HN
dotnet ef database update
dotnet run --launch-profile https
```

Mở `https://localhost:7110` hoặc `http://localhost:5058`. Chuỗi kết nối nằm trong `appsettings.json`.

## Tài khoản dùng thử

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Admin | `admin1` | `Admin@123` |
| Nhân viên | `nhanvien1` | `NhanVien@123` |
| Khách hàng | `khach1` | `KhachHang@123` |

Dữ liệu phòng, khách hàng và đặt phòng mẫu được nạp tự động ở lần chạy đầu.

## Tác giả

**Lê Văn Thanh** – MSV 23103100200 – Lớp TI17A4HN, UNETI.
