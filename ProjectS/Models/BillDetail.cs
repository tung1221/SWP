    namespace Project.Models
{
    public class BillDetail
    {   
        public int BillDetailId { get; set; }
        public int BillId { get; set; }
        public int ProductId { get; set; }
        public int quantity { get; set; }
        public virtual Bill Bill { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;

    }
}
