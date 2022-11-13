using System.ComponentModel.DataAnnotations.Schema;
using static Sheca.Common.Enum;

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
        [Column(TypeName = "varchar(20)")]
        public string DayOfWeeks { get; set; } = string.Empty;
        public int NumOfLessonsPerDay { get; set; }
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        public int NumOfLessons { get; set; }
        public int? NotiBeforeTime { get; set; }
        public NotificationUnit? NotiUnit { get; set; }
        public string ColorCode { get; set; }
        public string OffDays { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public List<DateTime> GetOffDaysList(){
            var offDays = OffDays.Split(";").Where(d => string.IsNullOrEmpty(d)).Select(d => new DateTime(long.Parse(d)));
            return offDays.ToList();
        }
    }
}
