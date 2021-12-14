using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherDataSmsService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWeatherDataService _weatherDataService;
        private readonly ISmsSenderService _smsSenderService;

        public Worker(ILogger<Worker> logger, IWeatherDataService weatherDataService, ISmsSenderService smsSenderService)
        {
            _logger = logger;
            _weatherDataService = weatherDataService;
            _smsSenderService = smsSenderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                WeatherData currentWeather = await _weatherDataService.GetCurrentWeather();
                string weatherInfo = ($"Current weather is {currentWeather.WeatherText}. It is {currentWeather.Temperature.Metric.Value} {currentWeather.Temperature.Metric.Unit} at {currentWeather.LocalObservationDateTime}");
                _logger.LogInformation(weatherInfo);
                _smsSenderService.SendSMS(weatherInfo);

                await Task.Delay(1000 * 60, stoppingToken);
            }
        }
    }
}
