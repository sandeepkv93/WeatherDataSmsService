using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace WeatherDataSmsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var settings = config.Build();
                    var env = hostContext.HostingEnvironment;

                    if (!env.IsDevelopment())
                    {
                        var keyVaultEndpoint = settings.GetValue<string>("AzureKeyVaultEndpoint");

                        if (!string.IsNullOrEmpty(keyVaultEndpoint))
                        {
                            var azureServiceTokenProvider = new AzureServiceTokenProvider();
                            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                            config.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                        }
                    } 
                    else
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }

                        var keyVaultEndpoint = settings.GetValue<string>("AzureKeyVaultEndpoint");
                        var keyVaultClientId = settings.GetValue<string>("AzureKeyVault:ClientId");
                        var keyVaultClientSecret = settings.GetValue<string>("AzureKeyVault:ClientSecret");

                        if (!string.IsNullOrEmpty(keyVaultEndpoint) && !string.IsNullOrEmpty(keyVaultClientId) && !string.IsNullOrEmpty(keyVaultClientSecret))
                        {
                            config.AddAzureKeyVault(keyVaultEndpoint, keyVaultClientId, keyVaultClientSecret, new DefaultKeyVaultSecretManager());
                        }
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;

                    string accuWeatherUrl = string.Format(config.GetValue<string>("AccuWeatherUrl"), config.GetValue<string>("AccWeatherApiKey"));
                    services.AddSingleton<IWeatherDataService>(weatherDataService => new WeatherDataService(new HttpClient(), accuWeatherUrl));

                    string twilioAccountSID = config.GetValue<string>("TwilioAccountSID");
                    string twilioAuthToken = config.GetValue<string>("TwilioAuthToken");
                    string twilioPhoneNumber = config.GetValue<string>("TwilioPhoneNumber");
                    string toPhoneNumber = config.GetValue<string>("ToPhoneNumber");
                    services.AddSingleton<ISmsSenderService>(smsSenderService => new SmsSenderService(twilioAccountSID, twilioAuthToken, twilioPhoneNumber, toPhoneNumber));

                    services.AddHostedService<Worker>();
                });
    }
}
