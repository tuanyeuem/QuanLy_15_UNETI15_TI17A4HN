# Kiểm thử Module 5

Chạy trên Windows có .NET 10 và SQL Server LocalDB:

```powershell
dotnet run --project RegressionTests/RegressionTests.csproj
```

Bộ kiểm thử tạo database riêng `QLKS_Regression_<GUID>` và xóa sau khi chạy,
không sử dụng database ứng dụng. Kiểm tra đổi phòng, trả sớm, lịch phòng của
booking cũ và hai yêu cầu đặt trùng đồng thời.

Nhánh `md5` giữ mô hình trạng thái độc lập của Module 5. Các controller của
Module 2–4 và sửa đổi tích hợp không thuộc nhánh này. Khi tích hợp, mọi luồng
thay đổi lịch phòng phải dùng cùng `GiaoDichLichPhong` trước khi đọc và lưu lịch.
