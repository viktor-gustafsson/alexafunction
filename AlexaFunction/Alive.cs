using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AlexaFunction;

public static class Alive
{
    [FunctionName("Alive")]
    public static async Task RunAsync([TimerTrigger("0 * * * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
    }
}