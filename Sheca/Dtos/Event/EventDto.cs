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
        public int? NotiBeforeTime { get; set; }
        public Guid? BaseEventId { get; set; }
        public Guid? CloneEventId { get; set; }
        public int? CourseId { get; set; }
        public CourseDto? Course { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }

        [SameExist("RecurringInterval", ErrorMessage = "RecurringStart and RecurringType must exist in same time")]
        public DateTime? RecurringStart { get; set; }
        [SameExist("RecurringUnit", ErrorMessage = "RecurringUnit and RecurringInterval must exist in same time")]
        public int? RecurringInterval { get; set; }
        public RecurringUnit? RecurringUnit { get; set; }
        public string? RecurringDetails { get; set; }
        public DateTime? RecurringEnd { get; set; }
    }
}
