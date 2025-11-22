using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HatiShop.Models
{
    public class BillDetail
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = string.Empty;

        [Key]
        [StringLength(50)]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public double Total { get; set; }

        // Navigation properties
        public virtual Bill Bill { get; set; }
        public virtual Product Product { get; set; }
    }
}