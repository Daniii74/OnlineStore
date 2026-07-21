using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;


        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}