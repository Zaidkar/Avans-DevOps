namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients
{
    public class SlackSdk
    {
        public void SendSlack(string channel, string body)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SlackSdk] SendSlack -> channel: {channel} | body: {body}");
        }
    }
}
