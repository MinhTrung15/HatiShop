using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HatiShop.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        [Required(ErrorMessage = "Mã KH là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Mã KH")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [StringLength(15)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? AvatarPath { get; set; }

        [Display(Name = "Doanh thu")]
        [Range(0, int.MaxValue, ErrorMessage = "Doanh thu không thể âm")]
        public double Revenue { get; set; }

        [Display(Name = "Hạng")]
        [StringLength(20)]
        public string Rank { get; set; } = "ĐỒNG";

        [NotMapped]
        [Display(Name = "Ảnh đại diện")]
        public IFormFile? AvatarFile { get; set; }

        // ✅ THÊM NAVIGATION PROPERTY NÀY
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}