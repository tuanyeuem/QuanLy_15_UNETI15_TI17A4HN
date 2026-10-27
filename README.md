# Module 5 – Chi tiết đặt phòng, tính tiền và thống kê

- **Sinh viên phụ trách:** Lê Văn Thanh
- **Mã sinh viên:** 23103100200
- **Đồ án:** Quản lý khách sạn – QuanLy_15_UNETI15_TI17A4HN
- **Nhánh:** `md5`
- **Nhánh tích hợp:** `maintest1`

Nhánh này chứa phần Module 5 trên nền ứng dụng chung của nhóm. Các module khác được giữ làm nền để chạy và tích hợp.

## Chức năng Module 5

- Quản lý chi tiết đặt phòng: danh sách, xem chi tiết, thêm, sửa và xóa theo điều kiện nghiệp vụ.
- Tìm kiếm theo khách hàng, số phòng, loại phòng; lọc trạng thái và ngày; sắp xếp, phân trang.
- Tính số ngày lưu trú, đơn giá và thành tiền phòng; chốt kết quả, trừ tiền cọc để hiển thị số tiền còn phải thu hoặc hoàn lại.
- Cho khách hàng xem kết quả đặt phòng của chính mình.
- Dashboard tổng quan phòng, khách hàng, đặt phòng và doanh thu tiền phòng đã chốt.
- Thống kê, báo cáo doanh thu theo khoảng ngày và xuất CSV.

## Các file chính

Các đường dẫn dưới đây nằm trong thư mục `QuanLy_15_UNETI15_TI17A4HN/`:

- `Models/ChiTietDatPhong.cs`: entity chi tiết đặt phòng và trạng thái lưu trú.
- `Controllers/ChiTietDatPhongController.cs`: xử lý chi tiết, tính tiền và chốt kết quả.
- `Controllers/ThongKeController.cs`: dashboard, thống kê và báo cáo.
- `ViewModels/ChiTietDatPhongViewModels.cs`, `ViewModels/ThongKeViewModels.cs`: dữ liệu cho các màn hình.
- `Views/ChiTietDatPhong/`, `Views/ThongKe/`: giao diện Module 5.
- `Helpers/TinhTienHelper.cs`: tính số ngày và tiền phòng.
- `Helpers/GiaoDichLichPhong.cs`: khóa giao dịch trong SQL Server khi thay đổi lịch phòng.
- `Data/AppDbContext.cs`, `Data/DuLieuMauGiaoDich.cs`: cấu hình dữ liệu dùng chung và dữ liệu giao dịch mẫu.

## Quy tắc nghiệp vụ và sửa lỗi

- Đơn giá và thành tiền do hệ thống tính; giữ đơn giá đã lưu nếu không đổi phòng.
- Kiểm tra khoảng ngày hợp lệ và lịch đặt trùng trước khi lưu.
- Khi đổi phòng chính trong chi tiết, cập nhật phòng đại diện của lượt đặt.
- Ưu tiên lịch trong chi tiết lưu trú; chỉ dùng lịch phòng chính cho booking cũ chưa có chi tiết. Khách trả sớm không bị giữ lịch đến ngày trả dự kiến.
- Các luồng thay đổi đặt phòng dùng chung khóa giao dịch để tránh hai yêu cầu đồng thời lưu lịch trùng.
- Chỉ chốt kết quả khi khách đã nhận phòng và các chi tiết còn hiệu lực đủ điều kiện; sau khi chốt, giữ lịch sử và cập nhật trạng thái phòng.
- Admin và nhân viên được quản lý; khách hàng chỉ xem dữ liệu thuộc tài khoản của mình.

Các điều chỉnh liên quan trong `DatPhongController`, `KhachHangController` và `PhongController` phục vụ việc đồng bộ lịch phòng với Module 5.

## Môi trường và cách chạy

Cần **.NET SDK 10**, **SQL Server LocalDB** hoặc SQL Server. Dự án sử dụng **ASP.NET Core MVC**, **Entity Framework Core 10** và Razor Views.

Từ thư mục gốc repository:

```powershell
cd QuanLy_15_UNETI15_TI17A4HN
dotnet restore
dotnet run
```

Kiểm tra `ConnectionStrings:DefaultConnection` trong `appsettings.json` trước khi chạy. Kết nối mặc định:

```text
Server=(localdb)\MSSQLLocalDB;Database=QuanLyKhachSan;Trusted_Connection=True;TrustServerCertificate=True
```

Ứng dụng chạy migration và nạp dữ liệu mẫu khi khởi động. Mở địa chỉ hiển thị trong terminal.

Các màn hình chính sau khi đăng nhập đúng vai trò:

- `/ChiTietDatPhong`: danh sách chi tiết đặt phòng.
- `/ChiTietDatPhong/KetQuaCuaToi`: kết quả của khách hàng đang đăng nhập.
- `/ThongKe/Dashboard`: dashboard.
- `/ThongKe`: thống kê.
- `/ThongKe/BaoCao`: báo cáo doanh thu.

## Tích hợp

Tạo pull request từ **`md5` vào `maintest1`** để đưa các thay đổi vào bản chung. Nhánh đã được đồng bộ nền `maintest1` tại thời điểm cập nhật; cần kiểm tra lại xung đột nếu nhánh đích có thay đổi mới.
