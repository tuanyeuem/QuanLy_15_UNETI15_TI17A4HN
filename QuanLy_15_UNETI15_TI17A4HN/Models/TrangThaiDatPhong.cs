namespace QuanLy_15_UNETI15_TI17A4HN.Models;

public static class TrangThaiDatPhong
{
    // Trạng thái của luồng duyệt/nhận/trả phòng (Module 4).
    public const string ChoDuyet = "ChoDuyet";
    public const string DaDuyet = "DaDuyet";
    public const string DaNhanPhong = "DaNhanPhong";
    public const string DaTraPhong = "DaTraPhong";
    public const string Huy = "Huy";

    // Trạng thái tổng hợp của luồng đặt nhiều phòng (Module 5).
    public const string ChoXuLy = "ChoXuLy";
    public const string DangXuLy = "DangXuLy";
    public const string HoanThanh = "HoanThanh";
    public const string DaHuy = "DaHuy";

    public static readonly string[] TatCa =
    {
        ChoDuyet, DaDuyet, DaNhanPhong, DaTraPhong, Huy,
        ChoXuLy, DangXuLy, HoanThanh, DaHuy
    };
}
