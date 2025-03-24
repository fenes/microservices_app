namespace ArticleService.Models.Common
{
    public class ApiError
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public string Details { get; set; }
        public string StackTrace { get; set; }

        public ApiError(string message, string code = null, string details = null, string stackTrace = null)
        {
            Message = message;
            Code = code;
            Details = details;
            StackTrace = stackTrace;
        }
    }
} 