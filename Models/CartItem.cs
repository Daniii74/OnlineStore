using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }


        [Required]
        public string UserId { get; set; }


        public int ProductId { get; set; }


        public int Quantity { get; set; }


        public Product? Product { get; set; }
    }
}
