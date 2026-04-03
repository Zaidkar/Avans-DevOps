using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class NotificationChannelFactory(
    IMailProvider mailProvider,
    ISlackProvider slackProvider,
    ISmsProvider smsProvider) : INotificationChannelFactory
{
    private readonly IMailProvider _mailProvider = mailProvider;
    private readonly ISlackProvider _slackProvider = slackProvider;
    private readonly ISmsProvider _smsProvider = smsProvider;

    public INotificationChannel Create(ChannelType channelType)
    {
        return channelType switch
        {
            ChannelType.Email => new EmailNotificationChannel(_mailProvider),
            ChannelType.Slack => new SlackNotificationChannel(_slackProvider),
            ChannelType.Sms => new SmsNotificationChannel(_smsProvider),
            _ => throw new ArgumentOutOfRangeException(nameof(channelType), channelType, "Unsupported channel type")
        };
    }
}
