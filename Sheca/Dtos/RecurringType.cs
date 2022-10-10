using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class RecurringType
    {
        public int Value{ get; set; }
        public RecurringUnit RecurringUnit { get; set; }
        public string? Details{ get; set; }
    }
}
