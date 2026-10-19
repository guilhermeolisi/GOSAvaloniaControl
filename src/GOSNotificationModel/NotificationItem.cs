using System.Windows.Input;

namespace GOSAvaloniaControls;

public class NotificationItem
{
    public NotificationItem(byte severity, string message, bool showBallon)
    {
        Severity = severity;
        Message = message;
        ShowBallon = showBallon;
    }

    /// <summary>
    /// Notificacao com uma acao (E175): o balao e a lista do sino mostram um botao com <paramref name="actionText"/> que
    /// executa <paramref name="action"/>, como o "Open results" do fim de um run.
    /// </summary>
    public NotificationItem(byte severity, string message, bool showBallon, string actionText, Action action)
        : this(severity, message, showBallon)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actionText);
        ArgumentNullException.ThrowIfNull(action);
        ActionText = actionText;
        ActionCommand = new NotificationActionCommand(action);
    }

    public byte Severity { get; set; }
    public string Message { get; set; }
    public bool ShowBallon { get; set; }

    /// <summary>Texto do botao de acao; nulo quando a notificacao nao tem acao.</summary>
    public string? ActionText { get; }

    /// <summary>Comando do botao de acao; nulo quando a notificacao nao tem acao.</summary>
    public ICommand? ActionCommand { get; }

    public bool HasAction => ActionCommand is not null;

    /// <summary>Comando sempre executavel que roda a acao da notificacao.</summary>
    private sealed class NotificationActionCommand(Action action) : ICommand
    {
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => action();
    }
}
