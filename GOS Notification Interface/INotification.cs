namespace GOSAvaloniaControls;

public interface INotification
{
    void AddNotification(byte severity, string message, bool showBallon);
}
