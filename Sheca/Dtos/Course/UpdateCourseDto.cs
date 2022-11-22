using Sheca.Attributes;
using System.ComponentModel.DataAnnotations;
using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class UpdateCourseDto
    {
        public string? Title { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        //seconds
        [Range(0, 86400, ErrorMessage = "StartTime is between 0 - 86400")]
        public int? StartTime { get; set; }
        [Range(0, 86400, ErrorMessage = "EndTime is between 0 - 86400")]
        public int? EndTime { get; set; }
        public int? NumOfLessonsPerDay { get; set; }
        [MinLength(1)]
        public List<DayOfWeek>? DayOfWeeks { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumOfLessons { get; set; }
        [SameExist("NotiUnit", ErrorMessage = "NotiBeforeTime and NotiUnit must exist in same time")]
        public int? NotiBeforeTime { get; set; }
        public NotificationUnit? NotiUnit { get; set; }
        public string? ColorCode { get; set; }
        public EndDateCourseType? UpdateEndType
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return EndDateCourseType.EndDate;
                }

                if (NumOfLessons.HasValue)
                {
                    return EndDateCourseType.NumberOfLessons;
                }
                return null;
            }
        }
    }
}
