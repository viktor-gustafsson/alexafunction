using Alexa.NET.Request;
using Alexa.NET.Request.Type;

namespace AlexaFunction
{
    public static class Extensions
    {
        public static bool IsLaunchRequest(this SkillRequest skillRequest) =>
            skillRequest.GetType() == typeof(LaunchRequest);

        public static bool IsIntentRequest(this SkillRequest skillRequest) =>
            skillRequest.GetType() == typeof(IntentRequest);
    }
}