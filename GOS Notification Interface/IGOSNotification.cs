namespace GOSAvaloniaControls;

public interface IGOSNotification
{
    bool UIContextIsNull { get; }

    void AddNotification(byte severity, string message, bool showBallon);
    void SetUIContext(SynchronizationContext? uiContext);
}
