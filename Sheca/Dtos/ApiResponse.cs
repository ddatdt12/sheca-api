namespace Sheca.DTOs
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public ApiResponse(T data, string message)
        {
            this.Data= data;
            this.Message=message;
        }
    }
}
