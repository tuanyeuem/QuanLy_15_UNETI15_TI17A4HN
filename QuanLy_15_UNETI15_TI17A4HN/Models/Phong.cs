using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLy_15_UNETI15_TI17A4HN.Models
{
    public class Phong
    {
        [Key]
        [Required(ErrorMessage = "Mã phòng không được để trống")]
        [StringLength(20, MinimumLength = 2,
            ErrorMessage = "Mã phòng phải từ 2 đến 20 ký tự")]
        [RegularExpression(@"^[A-Za-z0-9]+$",
            ErrorMessage = "Mã phòng chỉ được chứa chữ cái và số")]
        [Display(Name = "Mã phòng")]
        public string MaPhong { get; set; }

        [Required(ErrorMessage = "Số phòng không được để trống")]
        [StringLength(10,
            ErrorMessage = "Số phòng không được vượt quá 10 ký tự")]
        [RegularExpression(@"^[0-9]+$",
            ErrorMessage = "Số phòng chỉ được chứa số")]
        [Display(Name = "Số phòng")]
        public string SoPhong { get; set; }

        [Required(ErrorMessage = "Mã loại phòng không được để trống")]
        [Display(Name = "Mã loại phòng")]
        public string MaLoaiPhong { get; set; }

        [Range(1, 100, ErrorMessage = "Tầng phải từ 1 đến 100")]
        [Display(Name = "Tầng")]
        public int Tang { get; set; }

        [Required(ErrorMessage = "Hướng phòng không được để trống")]
        [StringLength(50,
            ErrorMessage = "Hướng phòng không được vượt quá 50 ký tự")]
        [Display(Name = "Hướng phòng")]
        public string HuongPhong { get; set; }

        [Required(ErrorMessage = "Tiện nghi không được để trống")]
        [StringLength(500,
            ErrorMessage = "Tiện nghi không được vượt quá 500 ký tự")]
        [Display(Name = "Tiện nghi")]
        public string TienNghi { get; set; }

        [Range(typeof(decimal), "0.01", "1000000000",
            ErrorMessage = "Đơn giá phải lớn hơn 0 và không vượt quá 1 tỷ")]
        [Display(Name = "Đơn giá")]
        [DataType(DataType.Currency)]
        public decimal DonGia { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [StringLength(30,
            ErrorMessage = "Trạng thái không được vượt quá 30 ký tự")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }

        [StringLength(500,
            ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày bảo trì")]
        public DateTime? NgayBaoTri { get; set; }

        [StringLength(500,
            ErrorMessage = "Ghi chú bảo trì không được vượt quá 500 ký tự")]
        [Display(Name = "Ghi chú bảo trì")]
        public string? GhiChuBaoTri { get; set; }

        public virtual LoaiPhong? LoaiPhong { get; set; }
    }
}