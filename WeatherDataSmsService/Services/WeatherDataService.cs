using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherDataSmsService
{
    class WeatherDataService : IWeatherDataService
    {
        public WeatherDataService(HttpClient httpClient, string accuWeatherUrl)
        {
            this.httpClient = httpClient;
            this.accuWeatherUrl = accuWeatherUrl;
        }

        private readonly HttpClient httpClient;
        private readonly string accuWeatherUrl;

        public async Task<WeatherData> GetCurrentWeather()
        {
            List<WeatherData> weatherInfo = await JsonSerializer.DeserializeAsync<List<WeatherData>>(await httpClient.GetStreamAsync(accuWeatherUrl));
            return weatherInfo.First();
        }
    }
}
