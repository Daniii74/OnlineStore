using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }


        [Required]
        public string UserId { get; set; }


        public DateTime OrderDate { get; set; } = DateTime.Now;


        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }


        [MaxLength(50)]
        public string Status { get; set; } = "Pending";


        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Pending";


        public ICollection<OrderDetail> OrderDetails { get; set; }

        public Payment Payment { get; set; }
    }
}
