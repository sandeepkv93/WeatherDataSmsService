using System.Threading.Tasks;

namespace WeatherDataSmsService
{
    public interface IWeatherDataService
    {
        Task<WeatherData> GetCurrentWeather();
    }
}