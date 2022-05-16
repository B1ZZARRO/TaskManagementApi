namespace AssistantApi.ViewModels
{
    public class ApiResponse<T> : ApiResponseMessage
    {
        public T Body { get; set; }
        
        public ApiResponse(T body)
        {
            Body = body;
        }
        
        public ApiResponse(T body, string message) : base(message)
        {
            Body = body;
        }
    }
}