namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers
{
    public interface ISmsProvider
    {
        void SendSms(string number, string body);
    }
}
