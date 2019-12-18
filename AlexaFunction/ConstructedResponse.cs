namespace AlexaFunction
{
    public class ConstructedResponse
    {
        public string Message { get; }
        public bool KeepOpen { get; }

        public ConstructedResponse(string message, bool keepOpen = false)
        {
            Message = message;
            KeepOpen = keepOpen;
        }
    }
}