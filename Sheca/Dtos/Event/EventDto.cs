using Sheca.Attributes;
using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class EventDto
    {
        public EventDto()
        {
            Title = string.Empty;
            Description = string.Empty;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            ColorCode = "#1a73e8";
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ColorCode { get; set; }
        //minutes
        public Guid? BaseEventId { get; set; }
        public Guid? CloneEventId { get; set; }
        public int? CourseId { get; set; }
        public CourseDto? Course { get; set; }
        public Guid UserId { get; set; }

        [SameExist("NotiUnit", ErrorMessage = "NotiBeforeTime and NotiUnit must exist in same time")]
        public int? NotiBeforeTime { get; set; }     //minutes
        public NotificationUnit? NotiUnit { get; set; }
        
        public UserDto? User { get; set; }

        [SameExist("RecurringInterval", ErrorMessage = "RecurringStart and RecurringType must exist in same time")]
        public DateTime? RecurringStart { get; set; }
        [SameExist("RecurringUnit", ErrorMessage = "RecurringUnit and RecurringInterval must exist in same time")]
        public int? RecurringInterval { get; set; }
        public RecurringUnit? RecurringUnit { get; set; }
        public List<DayOfWeek>? RecurringDetails { get; set; }
        public DateTime? RecurringEnd { get; set; }
    }
}
