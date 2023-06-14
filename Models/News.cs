using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Group1_CourseOnline.Models
{
    public partial class News
    {
        public int NewsId { get; set; }
        public string NewsTitle { get; set; } = null!;
        public string NewsHeading { get; set; } = null!;
        public string? NewsImage { get; set; } 
        public string NewsContent { get; set; } = null!;

		public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; } 
        public string? ModifiedBy { get; set; }
        public int EmployeeId { get; set; }
        public int CategoryNewId { get; set; }

        public int Views { get; set; }
        public virtual NewCategory CategoryNew { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;
    }
}
