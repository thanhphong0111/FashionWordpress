using System;
using System.Collections.Generic;

namespace Group1_CourseOnline.Models
{
    public partial class Lesson
    {
        public Lesson()
        {
            LessonVideos = new HashSet<LessonVideo>();
        }

        public int LessonId { get; set; }
        public string LessonName { get; set; } = null!;
        public int ProductId { get; set; }

        public virtual ICollection<LessonVideo> LessonVideos { get; set; }
    }
}
