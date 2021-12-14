namespace WeatherDataSmsService
{
    public interface ISmsSenderService
    {
        void SendSMS(string content);
    }
}