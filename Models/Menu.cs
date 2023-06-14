using System;
using System.Collections.Generic;

namespace Group1_CourseOnline.Models
{
    public partial class Menu
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = null!;
        public string? Url { get; set; }
        public bool Status { get; set; }
    }
}
