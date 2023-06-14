using System;
using System.Collections.Generic;

namespace Group1_CourseOnline.Models
{
    public partial class LessonVideo
    {
        public int VideoId { get; set; }
        public string VideoName { get; set; } = null!;
        public byte[]? FileVideo { get; set; }
        public int LessonId { get; set; }

        public virtual Lesson Lesson { get; set; } = null!;
    }
}
