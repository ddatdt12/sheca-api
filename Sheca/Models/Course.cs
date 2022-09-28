using Sheca.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sheca.Models
{
    [Table("Course")]
    public class Course
    {
        public Course()
        {
            Title = string.Empty;
            Description = string.Empty;
            ColorCode = "#1a73e8";
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string? Code { get; set; }
        public string Description { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int NumOfLessonsPerDay { get; set; }
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        public int NumOfLessons { get; set; }
        public int? NotiBeforeTime { get; set; }
        public string ColorCode { get; set; }
    }
}
