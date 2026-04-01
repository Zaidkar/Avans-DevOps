namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers
{
    public interface IMailProvider
    {
        void SendEmail(string to, string subject, string body);
    }
}
