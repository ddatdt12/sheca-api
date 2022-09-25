using System.Text.Json;

namespace Sheca.Error
{
    public class ApiException : Exception
    {
        private string _message;
        public string[] Errors { get; set; }
        public new string Message
        {
            get => _message;
            set
            {
                _message = value;
            }
        }
        public ApiException(string message = "Server Error", string[]? errors = null) : base(message)
        {
            StatusCode = 500;
            _message = message;
            Errors = errors != null ? errors : new string[] { message };
        }
        public ApiException(string message = "Server Error", int statusCode = 500, string[]? errors = null) : base(message)
        {
            StatusCode = statusCode;
            _message = message;
            Errors = errors != null ? errors : new string[] { message };
        }

        public int StatusCode { get; set; }
        public override string ToString() => JsonSerializer.Serialize(new
        {
            message = Message,
            statusCode = StatusCode,
            errors = Errors
        });

    }
}
