using Alexa.NET.Request;
using Alexa.NET.Request.Type;

namespace AlexaFunction
{
    public static class Extensions
    {
        public static bool IsLaunchRequest(this SkillRequest skillRequest) =>
            skillRequest.GetRequestType() == typeof(LaunchRequest);

        public static bool IsIntentRequest(this SkillRequest skillRequest) =>
            skillRequest.GetRequestType() == typeof(IntentRequest);
    }
}