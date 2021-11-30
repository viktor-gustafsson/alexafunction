using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.Security.Functions;
using AlexaFunction.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AlexaFunction;

public static class Alexa
{
    private const string NextNumberOfDeparturesCommand = "nextnumberofdepartures";
    private const string NextDepartureCommand = "nextdeparture";
    private const string AllDepartureCommand = "alldepartures";
    private const string DeviationCommand = "deviation";
    private static UserStationDataRepository _userStationDataRepository;
    private static string _userId;

    [FunctionName("Alexa")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        var json = await req.ReadAsStringAsync();
        var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);
            
        _userId = skillRequest.Context.System.User.UserId;

        if (!await skillRequest.ValidateRequestAsync(req, log))
            return new BadRequestResult();
            
        _userStationDataRepository = new UserStationDataRepository();
        var userStationData =
            await _userStationDataRepository.InsertUserIfNotExists(_userId);

        SkillResponse skillResponse;
        if (skillRequest.IsLaunchRequest())
        {
            skillResponse = ResponseBuilder.Tell(FormatHelper.GetOutputForSkillLaunch());
            skillResponse.Response.ShouldEndSession = false;
            return new OkObjectResult(skillResponse);
        }

        if (skillRequest.IsSessionEndRequest())
        {
            skillResponse = ResponseBuilder.Tell("Bye");
            skillResponse.Response.ShouldEndSession = true;
            return new OkObjectResult(skillResponse);
        }

        if (!skillRequest.IsIntentRequest())
            return new BadRequestResult();
            
        var response = await HandleIntent(skillRequest, userStationData);
        skillResponse = ResponseBuilder.Tell(response.Message);
        skillResponse.Response.ShouldEndSession = response.EndSession;

        return new OkObjectResult(skillResponse);
    }

    private static async Task<ConstructedResponse> HandleIntent(SkillRequest skillRequest, UserStationData userStationData)
    {
        var apiService = new ApiService();
            
        var intentRequest = skillRequest.Request as IntentRequest;
        var intentName = intentRequest?.Intent.Name.ToLowerInvariant();

        switch (intentName)
        {
            case "amazon.cancelintent":
                return new ConstructedResponse("Bye");
            case "amazon.helpintent":
                return new ConstructedResponse("Try asking for, next train departure.", false);
            case "amazon.stopintent":
                return new ConstructedResponse("Bye");
        }

        var departureData = await apiService.GetDepartureData(apiService, userStationData);
        var trainDepartureTimes = departureData.Trip
            .Select(trips => trips.LegList.Leg.FirstOrDefault()?.Origin.time).ToList();

        var timesToDeparture = FormatHelper.GetTimeToDeparture(trainDepartureTimes, FormatHelper.GetCurrentTime()).ToList();

        switch (intentName)
        {
            case NextNumberOfDeparturesCommand when intentRequest.Intent.Slots.Count == 1:
                return new ConstructedResponse(FormatHelper.GetOutputForNumberOfDepartures(intentRequest, timesToDeparture, userStationData)); 
            case AllDepartureCommand:
                return new ConstructedResponse($"Departure from {userStationData.FromStation} are,".GetOutputForDepartures(timesToDeparture));
            case NextDepartureCommand:
                return new ConstructedResponse($"Next train leaves {userStationData.FromStation} at {timesToDeparture.FirstOrDefault()}");
            case DeviationCommand:
                return new ConstructedResponse(FormatHelper.GetDeviationOutput(departureData));
            default:
                return new ConstructedResponse("I did not understand that");
        }
    }
}