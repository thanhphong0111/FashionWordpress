using System;
using System.Collections.Generic;

namespace Group1_CourseOnline.Models
{
    public partial class NewCategory
    {
        public NewCategory()
        {
            News = new HashSet<News>();
        }

        public int NewCategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
