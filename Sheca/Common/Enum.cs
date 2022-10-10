using System.Text.Json.Serialization;

namespace Sheca.Common
{
    public static class Enum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderStatus
        {
            Created,
            Doing,
            Delivered,  
            Completed,
            Cancelled,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TargetType
        {
            THIS,
            THIS_AND_FOLLOWING,
            ALL,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum RecurringUnit
        {
            DAY = 86400,
            WEEK = 604800,
        }
    }
}
