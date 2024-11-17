namespace TestCase.Models.ViewModels
{
    public class ErrorResponse
    {
        public string Message { get; }
        public ErrorResponse(string message)
        {
            Message = message;
        }
    }
}
