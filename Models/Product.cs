using System;
using System.Collections.Generic;

namespace Group1_CourseOnline.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public int? CategoryId { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitOfQuantitySold { get; set; }
        public int? NumberSession { get; set; }
        public bool Discontinued { get; set; }
      
        public string? Image { get; set; }
        public string? Curriculum { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifireDate { get; set; }
        public string? ModifireBy { get; set; }
        public int EmployeeId { get; set; }

        public int Views { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Employee Employee { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
