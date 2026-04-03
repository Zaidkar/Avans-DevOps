namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;

public interface INotificationStrategyFactory
{
    INotificationStrategy Create(ChannelType channelType);
}