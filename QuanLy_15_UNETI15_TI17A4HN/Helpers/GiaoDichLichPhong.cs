using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLy_15_UNETI15_TI17A4HN.Data;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class GiaoDichLichPhong
{
    // Khóa chung trong SQL Server, dùng được cả khi chạy nhiều instance ứng dụng.
    // Phải lấy khóa trước khi đọc lịch/trạng thái và giữ đến khi lưu xong.
    public static async Task<IDbContextTransaction> BatDauAsync(AppDbContext db)
    {
        var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("""
                DECLARE @result int;
                EXEC @result = sys.sp_getapplock
                    @Resource = N'QLKS:LichPhong',
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 15000;
                IF @result < 0
                    THROW 51000, N'Không thể khóa lịch phòng. Vui lòng thử lại.', 1;
                """);
            return transaction;
        }
        catch
        {
            await transaction.DisposeAsync();
            throw;
        }
    }
}
