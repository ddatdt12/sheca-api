using System.Text.Json.Serialization;

namespace Sheca.Common
{
    public static class Enum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserType
        {
            Admin,
            User
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Mode
        {
            Buyer,
            Seller
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PackageType
        {
            Basic,
            Standard,
            Premium
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PostStatus
        {
            Draft,
            PendingApproval,
            Active,
            Paused,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderStatus
        {
            Created,
            Doing,
            Delivered,  
            Completed,
            Cancelled,
        }
    }
}
