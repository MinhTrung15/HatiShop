using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HatiShop.Models
{
    public class Product
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public double Cost { get; set; }
        public double Price { get; set; }



        [StringLength(50)]
        public string? Type { get; set; }

        public int Quantity { get; set; }

        [StringLength(10)]
        public string? Size { get; set; }

        public string? Info { get; set; }

        public string? AvatarPath { get; set; }

        // Navigation property - ĐỔI TÊN
        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
    }
}