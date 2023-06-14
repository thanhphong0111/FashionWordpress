using System;
using System.Collections.Generic;

namespace Group1_CourseOnline.Models
{
    public partial class Comment
    {
        
        public int CommentId { get; set; }
        public string Content { get; set; } = null!;
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CommentTime { get; set; }
        public int? Rating { get; set; }
        public int EmployeeId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;
    }
}
