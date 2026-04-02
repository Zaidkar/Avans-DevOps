using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Publishers
{
    // Design Pattern: Observer. Publishes events to subscribed observers.
    public class NotificationPublisher
    {
        private readonly List<INotificationSubscriber> _subscribers = new();

        public void Subscribe(INotificationSubscriber subscriber)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [NotificationPublisher] Subscribe -> {subscriber.GetType().Name}");
            _subscribers.Add(subscriber);
        }

        public void Unsubscribe(INotificationSubscriber subscriber)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [NotificationPublisher] Unsubscribe -> {subscriber.GetType().Name}");
            _subscribers.Remove(subscriber);
        }

        public void Publish(INotificationEvent notificationEvent)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [NotificationPublisher] Publish -> {notificationEvent.GetType().Name} to {_subscribers.Count} subscriber(s)");

            foreach (var subscriber in _subscribers)
            {
                subscriber.Update(notificationEvent);
            }
        }
    }
}