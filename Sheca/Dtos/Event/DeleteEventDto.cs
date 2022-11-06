using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class DeleteEventDto
    {
        public DeleteEventDto()
        {
            TargetType = TargetType.THIS;
        }
        public Guid? Id { get; set; }
        public Guid? BaseEventId { get; set; }
        public Guid CloneEventId { get; set; }
        public DateTime StartTime{ get; set; }
        public TargetType TargetType { get; set; }
    }
}
