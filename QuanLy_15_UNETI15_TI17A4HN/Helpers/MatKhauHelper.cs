using System.Security.Cryptography;
using System.Text;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class MatKhauHelper
{
    public static string BamMatKhau(string matKhau)
    {
        if (string.IsNullOrWhiteSpace(matKhau))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(matKhau);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    public static bool KiemTraMatKhau(string matKhauNhap, string matKhauDaLuu)
    {
        return string.Equals(BamMatKhau(matKhauNhap), matKhauDaLuu, StringComparison.OrdinalIgnoreCase);
    }
}
