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
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ColorCode { get; set; }
        //minutes
        public int? NotiBeforeTime { get; set; }
        public Guid? BaseEvent { get; set; }
        public int? CourseId { get; set; }
        public CourseDto? Course { get; set; }
        public DateTime? RecurringStart { get; set; }
        public int? RecurringInterval { get; set; }
        public DateTime? RecurringEnd { get; set; }
    }
}
