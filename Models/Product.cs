using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class Product
    {
        public int ProductId { get; set; }


        [Required]
        public int CategoryId { get; set; }


        [Required]
        [MaxLength(150)]
        public string Name { get; set; }


        public string Description { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }


        public int Stock { get; set; }


        public string? ImageUrl { get; set; }


        public bool IsActive { get; set; } = true;


        public DateTime CreatedDate { get; set; } = DateTime.Now;



        public Category? Category { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
