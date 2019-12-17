using System;
using System.Collections.Generic;
using System.Linq;
using Alexa.NET.Request.Type;
using AlexaFunction.Models;

namespace AlexaFunction
{
    public static class FormatHelper
    {
        public static IEnumerable<string> GetTimeToDeparture(IEnumerable<string> departures, DateTime euTime)
        {
            var result = new List<string>();

            foreach (var departure in departures)
            {
                var departureTime = DateTime.Parse(departure);
                var duration = departureTime - euTime;
                result.Add(
                    $"{departure}, in {duration.Minutes.ToString()} minutes and {duration.Seconds.ToString()} seconds");
            }

            return result;
        }

        public static DateTime GetCurrentTime()
        {
            var euTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, euTimeZone);
        }

        public static string GetOutputForDepartures(this string s, IEnumerable<string> departures) =>
            departures.Aggregate(s, (current, departure) => current + $", {departure}");
        
        public static string GetOutputForNumberOfDepartures(IntentRequest intentRequest,
            IEnumerable<string> timeDifference)
        {
            var numberAsString = intentRequest.Intent.Slots.FirstOrDefault().Value.Value;
            var numberAsInt = int.Parse(numberAsString);
            if (numberAsInt > 5 || numberAsInt <= 0)
                return "You can only request between 1 and 5 departures";

            var subSetOfDepartures = timeDifference.Take(numberAsInt);
            return
                $"Next {numberAsString} departures from Norrviken station are,".GetOutputForDepartures(
                    subSetOfDepartures);
        }
        
        public static string GetDeviationOutput(RootObject departureData)
        {
            var tripMessageData = departureData.Trip.Select(trips =>
                trips.LegList.Leg.FirstOrDefault()?.Messages);

            var detectedIssuesCreatedToday = tripMessageData.Any(messages =>
                messages != null &&
                messages.Message.Any(message =>
                    message.sDate == FormatHelper.GetCurrentTime().ToString("yyyy-MM-dd")));

            return detectedIssuesCreatedToday
                ? "There might be deviations on your commute, check the app"
                : "There are no deviations on your commute";
        }
        public static string GetOutputForSkillLaunch() =>
            "Welcome to train commuter, try asking. When does the next train leave. Or, Tell me about train issues. " +
            "If you are done with train commuter you can close the skill by saying alexa stop";
    }
}