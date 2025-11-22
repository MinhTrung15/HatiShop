using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HatiShop.Models
{
    public class Bill
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = string.Empty; // ✅ ĐỔI THÀNH string

        [Required]
        [StringLength(50)]
        public string StaffId { get; set; } = string.Empty; // ✅ ĐỔI THÀNH string

        [Required]
        [StringLength(50)]
        public string CustomerId { get; set; } = string.Empty;

        public DateTime CreationTime { get; set; } = DateTime.Now;

        public double DiscountAmount { get; set; }
        public double OriginalPrice { get; set; }
        public double DiscountedTotal { get; set; }

        // Navigation properties
        public virtual Customer Customer { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
    }
}