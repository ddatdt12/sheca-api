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
        }
        [Required(ErrorMessage ="Title is required")]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ColorCode { get; set; }
        public int? NotiBeforeTime { get; set; }     //minutes

        [SameExist("RecurringInterval", ErrorMessage = "RecurringStart and RecurringType must exist in same time")]
        public DateTime? RecurringStart { get; set; }
        [SameExist("RecurringUnit", ErrorMessage = "RecurringUnit and RecurringInterval must exist in same time")]
        public int? RecurringInterval { get; set; }
        public RecurringUnit? RecurringUnit { get; set; }
        public string? RecurringDetails { get; set; }
        public DateTime? RecurringEnd { get; set; }
    }
}
