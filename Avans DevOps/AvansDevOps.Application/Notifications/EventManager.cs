namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class EventManager : IEventManager
{
    private readonly Dictionary<string, List<IEventListener>> _listeners = new(StringComparer.OrdinalIgnoreCase);

    public void Subscribe(string eventType, IEventListener listener)
    {
        if (!_listeners.ContainsKey(eventType))
        {
            _listeners[eventType] = new List<IEventListener>();
        }

        _listeners[eventType].Add(listener);
    }

    public void Unsubscribe(string eventType, IEventListener listener)
    {
        if (_listeners.TryGetValue(eventType, out var eventListeners))
        {
            eventListeners.Remove(listener);
        }
    }

    public void Notify(string eventType, NotificationEventData data)
    {
        if (!_listeners.TryGetValue(eventType, out var eventListeners))
        {
            return;
        }

        var eventData = string.IsNullOrWhiteSpace(data.EventType)
            ? new NotificationEventData
            {
                EventType = eventType,
                SprintId = data.SprintId,
                Subject = data.Subject,
                Body = data.Body
            }
            : data;

        foreach (var listener in eventListeners)
        {
            listener.Update(eventData);
        }
    }
}
