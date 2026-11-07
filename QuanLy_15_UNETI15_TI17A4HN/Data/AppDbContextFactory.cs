// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Tạo AppDbContext cho lệnh EF Core design-time.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace QuanLy_15_UNETI15_TI17A4HN.Data;
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var directory = new DirectoryInfo(Path.GetDirectoryName(typeof(AppDbContext).Assembly.Location)!);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            directory = directory.Parent;
        if (directory == null)
            throw new InvalidOperationException("Không tìm thấy appsettings.json để tạo migration.");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(directory.FullName)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;
        return new AppDbContext(options);
    }
}
