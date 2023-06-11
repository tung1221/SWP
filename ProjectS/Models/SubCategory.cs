namespace Project.Models
{
    public class SubCategory
    {
        public int SubCategoryId { get; set; }
        public string Name { get; set; } = null!;

        public virtual List<Product> Products { get; set; } = null!;
    }
}
