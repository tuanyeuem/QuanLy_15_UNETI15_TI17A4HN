// Họ và tên: Nguyễn Đình Tuấn
// Mã sinh viên: 23103100235
// Nội dung thực hiện: Băm mật khẩu bằng PasswordHasher; tương thích tài khoản SHA-256 cũ.
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class MatKhauHelper
{
    private static readonly PasswordHasher<object> Hasher = new();
    private static readonly object User = new();

    public static string BamMatKhau(string matKhau)
    {
        if (string.IsNullOrWhiteSpace(matKhau))
            return string.Empty;

        return Hasher.HashPassword(User, matKhau);
    }

    public static bool KiemTraMatKhau(string matKhauNhap, string matKhauDaLuu)
    {
        return KiemTraMatKhau(matKhauNhap, matKhauDaLuu, out _);
    }

    public static bool KiemTraMatKhau(string matKhauNhap, string matKhauDaLuu, out bool canNangCap)
    {
        canNangCap = false;
        if (string.IsNullOrWhiteSpace(matKhauNhap) || string.IsNullOrEmpty(matKhauDaLuu)) return false;
        try
        {
            if (matKhauDaLuu.Length == 64 && matKhauDaLuu.All(Uri.IsHexDigit))
            {
                var dung = CryptographicOperations.FixedTimeEquals(
                    SHA256.HashData(Encoding.UTF8.GetBytes(matKhauNhap)), Convert.FromHexString(matKhauDaLuu));
                canNangCap = dung;
                return dung;
            }
            var result = Hasher.VerifyHashedPassword(User, matKhauDaLuu, matKhauNhap);
            canNangCap = result == PasswordVerificationResult.SuccessRehashNeeded;
            return result != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
