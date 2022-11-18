using Sheca.Attributes;
using System.ComponentModel.DataAnnotations;
using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class CreateEventDto
    {
        public CreateEventDto()
        {
            Title = string.Empty;
            Description = string.Empty;
            ColorCode = "#1a73e8";
            //StartTime.DayOfWeek == DayOfWeek.Monday
        }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Description { get; set; }
        [LessThan("EndTime", ErrorMessage = "Endtime must be greater than Start Time")]
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ColorCode { get; set; }
        [SameExist("NotiUnit", ErrorMessage = "NotiBeforeTime and NotiUnit must exist in same time")]
        public int? NotiBeforeTime { get; set; }     //minutes
        public NotificationUnit? NotiUnit { get; set; }

        [MinValue(1)]
        [SameExist("RecurringUnit", ErrorMessage = "RecurringUnit and RecurringInterval must exist in same time")]
        public int? RecurringInterval { get; set; }

        public RecurringUnit? RecurringUnit { get; set; }
        public List<DayOfWeek>? RecurringDetails { get; set; }
        public DateTime? RecurringEnd { get; set; }
    }
}
