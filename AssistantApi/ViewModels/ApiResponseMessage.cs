namespace AssistantApi.ViewModels
{
    public class ApiResponseMessage
    {
        public string Message { get; set; }

        public ApiResponseMessage(string message)
        {
            Message = message;
        }
        
        public ApiResponseMessage()
        {
            Message = "Ok";
        }
    }
}