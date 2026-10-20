// Họ và tên: Lê Văn Thanh
// Mã sinh viên: 23103100200
// Nội dung thực hiện: Module 5 - Quy tắc tính số ngày ở và thành tiền phòng.
namespace QuanLy_15_UNETI15_TI17A4HN.Helpers;

public static class TinhTienHelper
{
    // Tính theo đêm: ở trong ngày (nhận và trả cùng ngày) vẫn tính tối thiểu 1 ngày.
    public static int TinhSoNgay(DateTime ngayNhan, DateTime ngayTra)
        => Math.Max(1, (ngayTra.Date - ngayNhan.Date).Days);

    public static decimal TinhThanhTien(decimal donGia, int soNgay)
        => donGia < 0 || soNgay < 0 ? 0 : donGia * soNgay;
}
