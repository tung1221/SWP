namespace Project.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int CategoryId { get; set; }
        public DateTime ImportDate { get; set; }
        public string ProductDescription { get; set; } = null!;
        public bool? typeGender { get; set; }
        public double PerDiscount { get; set; }
        public int ProductQuantities { get; set; }
        public double ProductPrice { get; set; }
        public int? BlogId { get; set; }
        public bool IsAvailble { get; set; } 
        public bool HomeStatus { get; set; }

        public int SubCategoryID { get; set; } 
        public string ListColor { get; set; } = null!;
        public string ImageMain { get; set; } = null!;
        public string ListSize { get; set; } = null!;
        public virtual Blog Blog { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;

        public virtual SubCategory SubCategory { get; set; } = null!;
        public virtual List<BillDetail> BillDetails { get; set; } = null!;
        public virtual List<Feedback> Feedbacks { get; set; } = null!;
        public virtual List<ImageProduct> ImageProducts { get; set; } = null!;
    }
}
