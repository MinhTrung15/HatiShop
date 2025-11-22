using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HatiShop.Models
{
    public class Staff
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public string? AvatarPath { get; set; }

        [StringLength(50)]
        public string? Role { get; set; }

        // ✅ THÊM PROPERTY NÀY
        [NotMapped]
        [Display(Name = "Ảnh đại diện")]
        public IFormFile? AvatarFile { get; set; }

        // Navigation property
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}