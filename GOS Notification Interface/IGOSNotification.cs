namespace GOSAvaloniaControls;

public interface IGOSNotification
{
    void AddNotification(byte severity, string message, bool showBallon);
}
