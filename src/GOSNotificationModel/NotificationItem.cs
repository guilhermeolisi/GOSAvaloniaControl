namespace GOSAvaloniaControls;

public class NotificationItem
{
    public NotificationItem(byte severity, string message, bool showBallon)
    {
        Severity = severity;
        Message = message;
        ShowBallon = showBallon;
    }

    public byte Severity { get; set; }
    public string Message { get; set; }
    public bool ShowBallon { get; set; }
}