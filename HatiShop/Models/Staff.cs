// Models/Staff.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HatiShop.Models
{
    public class Staff
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không quá 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Mật khẩu không quá 100 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Họ tên không quá 50 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [StringLength(4)]
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [StringLength(10, ErrorMessage = "Số điện thoại không quá 10 số")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không quá 100 ký tự")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? AvatarPath { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Nhân viên";

        [NotMapped]
        [Display(Name = "Ảnh đại diện")]
        public IFormFile? AvatarFile { get; set; }
    }
}