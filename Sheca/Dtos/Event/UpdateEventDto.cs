using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class UpdateEventDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? ColorCode { get; set; }
        //minutes
        public int? NotiBeforeTime { get; set; }
        public DateTime? RecurringStart { get; set; }
        public int? RecurringInterval { get; set; }
        public DateTime? RecurringEnd { get; set; }

        public Guid? Id { get; set; }
        public Guid? BaseEventId { get; set; }
        public Guid? CloneEventId { get; set; }
        public DateTime? BeforeStartTime { get; set; }
        public TargetType? TargetType{ get; set; }
    }
}
