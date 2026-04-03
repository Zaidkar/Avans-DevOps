namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public interface INotificationChannelFactory
{
    INotificationChannel Create(ChannelType channelType);
}
