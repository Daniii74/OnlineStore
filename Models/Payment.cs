using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }


        public int OrderId { get; set; }


        [MaxLength(50)]
        public string PaymentMethod { get; set; }


        [MaxLength(100)]
        public string TransactionId { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }


        public DateTime PaymentDate { get; set; } = DateTime.Now;


        [MaxLength(50)]
        public string Status { get; set; }


        public Order Order { get; set; }
    }
}
