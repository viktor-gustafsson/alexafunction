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
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AlexaFunction
{
    public static class Alexa
    {
        private const string NextNumberOfDeparturesCommand = "nextnumberofdepartures";
        private const string NextDepartureCommand = "nextdeparture";
        private const string AllDepartureCommand = "alldepartures";
        private const string DeviationCommand = "deviation";
        private static FireStore _fireStore;
        private static string _userId;

        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            var combine = System.IO.Path.Combine(context.FunctionDirectory, "..\\alexaskill-97042-5f96cc3da2d1.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", combine);
            
            var json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);
            
            _userId = skillRequest.Context.System.User.UserId;

            if (!await skillRequest.ValidateRequestAsync(req, log))
                return new BadRequestResult();
            
            _fireStore = new FireStore();
            var userStationData =
                await _fireStore.GetUserStationData(_userId);

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

            return intentName switch
            {
                NextNumberOfDeparturesCommand when intentRequest.Intent.Slots.Count == 1 => new ConstructedResponse(
                    FormatHelper.GetOutputForNumberOfDepartures(intentRequest, await GetDepartureTimes(userStationData),
                        userStationData)),
                AllDepartureCommand => new ConstructedResponse(
                    $"Departure from {userStationData.FromStation} are,".GetOutputForDepartures(
                        await GetDepartureTimes(userStationData))),
                NextDepartureCommand => new ConstructedResponse(
                    $"Next train leaves {userStationData.FromStation} at {(await GetDepartureTimes(userStationData)).FirstOrDefault()}"),
                DeviationCommand => new ConstructedResponse(await GetDeviatonInformation(userStationData)),
                _ => new ConstructedResponse("I did not understand that")
            };
        }

        private static async Task<string> GetDeviatonInformation(UserStationData userStationData)
        {
            var lastDeviationData = await _fireStore.GetLastDeviationData();
            if (lastDeviationData.Time.AddMinutes(30) < DateTime.UtcNow) 
                return lastDeviationData.Deviation;
            
            var apiService = new ApiService();
            var departureData = await apiService.GetDepartureData(apiService, userStationData);
            var deviationOutput = FormatHelper.GetDeviationOutput(departureData);
            await _fireStore.WriteDeviationData(deviationOutput);
            
            return deviationOutput;
        }
        
        private static async Task<List<string>> GetDepartureTimes(UserStationData userStationData)
        {
            var apiService = new ApiService();
            var departureData = await apiService.GetDepartureData(apiService, userStationData);
            var trainDepartureTimes = departureData.Trip
                .Select(trips => trips.LegList.Leg.FirstOrDefault()?.Origin.time).ToList();

            return FormatHelper.GetTimeToDeparture(trainDepartureTimes, FormatHelper.GetCurrentTime()).ToList();
        }
    }
}