using AlexaFunction.DAL;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AlexaFunction;

public static class Alive
{
    [FunctionName("Alive")]
    public static async Task RunAsync([TimerTrigger("0 */30 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        var combine = Path.Combine(context.FunctionDirectory, "..\\alexaskill-97042-5f96cc3da2d1.json");
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", combine);
        var apiService = new ApiService();
        var fireStore = new FireStore();
        var userStationData = await fireStore.GetFirst();

        var departureData = await apiService.GetDepartureData(apiService, userStationData);
        var deviationOutput = FormatHelper.GetDeviationOutput(departureData);
        
        await fireStore.WriteDeviationData(deviationOutput);
    }
}