using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class PostCourseDateOffDto
    {
        public DayOffAction Action { get; set; }
        public DateTime Date { get; set; }
    }
}
