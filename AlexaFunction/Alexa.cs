using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.Security.Functions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AlexaFunction
{
    public static class Alexa
    {
        private const string NextDeparturesCommand = "nextdepartures";
        private const string NextDepartureCommand = "nextdeparture";
        private const string DeviationCommand = "deviation";

        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);

            if (!await skillRequest.ValidateRequestAsync(req, log))
                return new BadRequestResult();

            SkillResponse skillResponse;
            if (skillRequest.IsLaunchRequest())
            {
                skillResponse = ResponseBuilder.Tell(FormatHelper.GetOutputForSkillLaunch());
                skillResponse.Response.ShouldEndSession = false;
                return new OkObjectResult(skillResponse);
            }

            if (!skillRequest.IsIntentRequest())
                return new BadRequestResult();

            skillResponse = ResponseBuilder.Tell(await HandleIntent(skillRequest));
            skillResponse.Response.ShouldEndSession = true;

            return new OkObjectResult(skillResponse);
        }

        private static async Task<string> HandleIntent(SkillRequest skillRequest)
        {
            var apiService = new ApiService();

            var intentRequest = skillRequest.Request as IntentRequest;
            var intentName = intentRequest?.Intent.Name.ToLowerInvariant();

            switch (intentName)
            {
                case "amazon.cancelintent":
                    return "";
                case "amazon.helpintent":
                    return "Try asking for, next train departures.";
                case "amazon.stopintent":
                    return "";
                case "amazon.fallbackintent":
                    return "I did not understand that";
            }

            var departureData = await apiService.GetDepartureData(apiService);
            var trainDepartureTimes = departureData.Trip
                .Select(trips => trips.LegList.Leg.FirstOrDefault()?.Origin.time).ToList();

            var timesToDeparture = FormatHelper.GetTimeToDeparture(trainDepartureTimes, FormatHelper.GetCurrentTime());

            switch (intentName)
            {
                case NextDeparturesCommand when intentRequest.Intent.Slots.Any():
                    return FormatHelper.GetOutputForNumberOfDepartures(intentRequest, timesToDeparture);
                case NextDeparturesCommand:
                    return "Departure from Norrviken station are,".GetOutputForDepartures(timesToDeparture);
                case NextDepartureCommand:
                    return $"Next train leaves Norrviken station at {timesToDeparture.FirstOrDefault()}";
                case DeviationCommand:
                    return FormatHelper.GetDeviationOutput(departureData);
                default:
                    return "I did not understand that";
            }
        }
    }
}