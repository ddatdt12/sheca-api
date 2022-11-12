using Sheca.Attributes;
using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class UpdateEventDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        [LessThan("EndTime", ErrorMessage = "Endtime must be greater than Start Time")]
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? ColorCode { get; set; }
        //minutes
        public int? NotiBeforeTime { get; set; }
        [SameExist("RecurringInterval", ErrorMessage = "RecurringStart and RecurringType must exist in same time")]
        public DateTime? RecurringStart { get; set; }
        [SameExist("RecurringUnit", ErrorMessage = "RecurringUnit and RecurringInterval must exist in same time")]
        public int? RecurringInterval { get; set; }
        public RecurringUnit? RecurringUnit { get; set; }
        public List<DayOfWeek>? RecurringDetails { get; set; }
        public DateTime? RecurringEnd { get; set; }
        public Guid? Id { get; set; }
        public Guid? BaseEventId { get; set; }
        public Guid? CloneEventId { get; set; }
        public DateTime? BeforeStartTime { get; set; }
        public TargetType? TargetType{ get; set; }
    }
}
