using System;
using System.Net.Http;
using System.Threading.Tasks;
using AlexaFunction.Models;
using Newtonsoft.Json;

namespace AlexaFunction
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _departureApiKey = Environment.GetEnvironmentVariable("apiKey");

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<RootObject> GetDepartureData(ApiService apiService)
        {
            const int bufferTimeInMinutes = 15;
            var searchTime = FormatHelper.GetCurrentTime().AddMinutes(bufferTimeInMinutes).ToString("HH:mm");
            var apiUrl =
                $"https://api.sl.se/api2/TravelplannerV3_1/trip.JSON?key={_departureApiKey}&lang=en&originExtId=Norrviken (Sollentuna)&destExtId=Odenplan (Stockholm)&time={searchTime}";
            return await apiService.GetTrainDepartures(apiUrl);
        }
        
        private async Task<RootObject> GetTrainDepartures(string url) =>
            JsonConvert.DeserializeObject<RootObject>(await _httpClient.GetStringAsync(url));
    }
}