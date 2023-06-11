namespace Project.Models
{
    public class Blog
    {
        public int Blogid { get; set; }
        public bool? HomeStatus { get; set; }
        public string content { get; set; } = null!;
        public virtual List<ImageBlog> ImageBlogs { get; set; } = null!;
        public virtual List<Product> Products { get; set; } = null!;
    }
}
