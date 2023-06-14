using System;
using System.Collections.Generic;

namespace Group1_CourseOnline.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public byte[]? Picture { get; set; }
        public string CreateBy { get; set; } = null!;
        public string? ModifireBy { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? ModifireTime { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
