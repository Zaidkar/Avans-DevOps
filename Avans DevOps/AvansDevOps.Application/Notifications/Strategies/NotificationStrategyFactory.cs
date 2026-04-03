using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;

public class NotificationStrategyFactory(
    ExternalMailClient mailClient,
    SlackSdk slackSdk,
    SmsSdk smsSdk) : INotificationStrategyFactory
{
    private readonly ExternalMailClient _mailClient = mailClient;
    private readonly SlackSdk _slackSdk = slackSdk;
    private readonly SmsSdk _smsSdk = smsSdk;

    public INotificationStrategy Create(ChannelType channelType)
    {
        return channelType switch
        {
            ChannelType.Email => new EmailNotificationStrategy(_mailClient),
            ChannelType.Slack => new SlackNotificationStrategy(_slackSdk),
            ChannelType.Sms => new SmsNotificationStrategy(_smsSdk),
            _ => throw new ArgumentOutOfRangeException(nameof(channelType), channelType, "Unsupported channel type")
        };
    }
}