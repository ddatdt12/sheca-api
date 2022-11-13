using static Sheca.Common.Enum;

namespace Sheca.Dtos
{
    public class FilterEvent
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TypeQuery Type { get; set; } = TypeQuery.ALL;
    }
}
