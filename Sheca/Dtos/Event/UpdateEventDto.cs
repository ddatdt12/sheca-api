using Sheca.Attributes;
using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class UpdateEventDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        //[SameExist("StartTime", ErrorMessage = "BeforeStartTime and StartTime must exist in same time")]
        public DateTime? BeforeStartTime { get; set; }
        [LessThan("EndTime", ErrorMessage = "Endtime must be greater than Start Time")]
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? ColorCode { get; set; }

        [SameExist("NotiUnit", ErrorMessage = "NotiBeforeTime and NotiUnit must exist in same time")]
        public int? NotiBeforeTime { get; set; }     //minutes
        public NotificationUnit? NotiUnit { get; set; }
        public DateTime? RecurringStart { get; set; }
        [MinValue(1)]
        public int? RecurringInterval { get; set; }
        public RecurringUnit? RecurringUnit { get; set; }
        public List<DayOfWeek>? RecurringDetails { get; set; }
        public DateTime? RecurringEnd { get; set; }
        public Guid? Id { get; set; }
        public Guid? BaseEventId { get; set; }
        public Guid? CloneEventId { get; set; }
        public TargetType? TargetType { get; set; }
        public bool HasRecurringChanged()
        {
            return RecurringStart.HasValue || RecurringEnd.HasValue || RecurringUnit.HasValue || RecurringInterval.HasValue || RecurringDetails != null && RecurringDetails.Any();
        }
    }
}
