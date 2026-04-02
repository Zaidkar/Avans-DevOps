using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Adapters
{
    // Design Pattern: Adapter. Wraps Slack SDK behind ISlackProvider.
    public class SlackApiAdapter(SlackSdk slackSdk) : ISlackProvider
    {
        private readonly SlackSdk _slackSdk = slackSdk;

        public void SendSlack(string channel, string body)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SlackApiAdapter] SendSlack -> {channel}");
            _slackSdk.SendSlack(channel, body);
        }
    }
}
