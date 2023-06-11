using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string UserId { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string Town { get; set; } = null!;
        public string District { get; set; } = null!;
        public virtual IdentityUser User { get; set; } = null!;
        public bool HomeStatus { get; set; }
    }
}
