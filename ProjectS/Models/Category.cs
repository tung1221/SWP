namespace Project.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public virtual List<Product> Products { get; set; } = null!;
    }
}
