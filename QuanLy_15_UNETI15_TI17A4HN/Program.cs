// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Cấu hình ứng dụng: DbContext SQL Server, Session, routing và nạp dữ liệu mẫu khi khởi động.
using Microsoft.EntityFrameworkCore;
using QuanLy_15_UNETI15_TI17A4HN.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // A legacy database may already have the complete schema from the old
    // 20261013171529 migration, which is no longer in this merged source tree.
    // Adopt the overlapping migration only when its recorded predecessor and
    // every table it creates are present; otherwise let EF apply it normally.
    db.Database.ExecuteSqlRaw("""
        IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
        AND EXISTS (SELECT 1 FROM [__EFMigrationsHistory]
                    WHERE [MigrationId] = N'20261013171529_TaoCSDLDayDuKhachSan')
           AND NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory]
                           WHERE [MigrationId] = N'20261006012500_Them_ChiTietDatPhong_VaCacEntityNen')
           AND OBJECT_ID(N'[dbo].[DichVus]', N'U') IS NOT NULL
           AND OBJECT_ID(N'[dbo].[KhachHangs]', N'U') IS NOT NULL
           AND OBJECT_ID(N'[dbo].[Phongs]', N'U') IS NOT NULL
           AND OBJECT_ID(N'[dbo].[DatPhongs]', N'U') IS NOT NULL
           AND OBJECT_ID(N'[dbo].[ChiTietDatPhongs]', N'U') IS NOT NULL
        BEGIN
            INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
            VALUES (N'20261006012500_Them_ChiTietDatPhong_VaCacEntityNen', N'10.0.13');
        END;
        """);

    db.Database.Migrate();
    DbSeeder.Seed(db);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
