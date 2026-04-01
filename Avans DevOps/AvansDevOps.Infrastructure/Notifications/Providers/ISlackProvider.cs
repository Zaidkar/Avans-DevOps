namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers
{
    public interface ISlackProvider
    {
        void SendSlack(string channel, string body);
    }
}
