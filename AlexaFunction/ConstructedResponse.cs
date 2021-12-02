namespace AlexaFunction;

public class ConstructedResponse
{
    public string Message { get; }
    public bool EndSession { get; }

    public ConstructedResponse(string message, bool endSession = true)
    {
        Message = message;
        EndSession = endSession;
    }
}