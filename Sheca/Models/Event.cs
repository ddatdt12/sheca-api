using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sheca.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using static Sheca.Common.Enum;

namespace Sheca.Models
{
    [Table("Event")]
    public class Event
    {
        public Event()
        {
            Id = Guid.NewGuid();
            Title = string.Empty;
            Description = string.Empty;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            ColorCode = "#1a73e8";
            ExceptDates = string.Empty;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ColorCode { get; set; }
        //minutes
        public int? NotiBeforeTime { get; set; }
        public NotificationUnit? NotiUnit { get; set; }
        public Guid? BaseEventId { get; set; }
        public Event? BaseEvent { get; set; }
        [NotMapped]
        public Guid? CloneEventId { get; set; }
        [NotMapped]
        public int? CourseId { get; set; }
        public Guid UserId { get; set; }
        public User? User{ get; set; }
        public DateTime? RecurringStart { get; set; }
        public int? RecurringInterval { get; set; }
        public RecurringUnit? RecurringUnit { get; set; }
        public string? RecurringDetails{ get; set; }
        public DateTime? RecurringEnd { get; set; }
        public string ExceptDates { get; set; }

        public List<DayOfWeek> GetRecurringDetails()
        {
            return RecurringDetails?.Split(';').Select(d => (DayOfWeek)int.Parse(d)).ToList() ?? new List<DayOfWeek>();
        }
        public List<string> GetExceptDates()
        {
            var exceptDates = ExceptDates.Split(";").Where(d => !string.IsNullOrEmpty(d)).ToList();
            return exceptDates;
        }
        public Event Clone()
        {
            var clone = (Event)MemberwiseClone();
            return clone;
        }
        public Event SimpleClone()
        {
            var clone = (Event)MemberwiseClone();
            clone.ExceptDates = "";
            clone.RecurringInterval = null;
            clone.RecurringDetails = null;
            clone.RecurringUnit  = null;
            clone.RecurringStart = null;
            clone.RecurringEnd = null;
            return clone;
        }
    }


    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasOne(e => e.BaseEvent).WithMany().HasForeignKey(e => e.BaseEventId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
