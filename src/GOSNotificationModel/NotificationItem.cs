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

    /// <summary>
    /// Notificacao com duas acoes (E183, lei 6.7): o balao mostra so a primeira; a lista do sino, as duas.
    /// </summary>
    public NotificationItem(byte severity, string message, bool showBallon, string actionText, Action action,
        string secondActionText, Action secondAction)
        : this(severity, message, showBallon, actionText, action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(secondActionText);
        ArgumentNullException.ThrowIfNull(secondAction);
        SecondActionText = secondActionText;
        SecondActionCommand = new NotificationActionCommand(secondAction);
    }

    public byte Severity { get; set; }
    public string Message { get; set; }
    public bool ShowBallon { get; set; }

    /// <summary>Texto do botao de acao; nulo quando a notificacao nao tem acao.</summary>
    public string? ActionText { get; }

    /// <summary>Comando do botao de acao; nulo quando a notificacao nao tem acao.</summary>
    public ICommand? ActionCommand { get; }

    public bool HasAction => ActionCommand is not null;

    /// <summary>Texto da segunda acao, so na lista do sino; nulo quando nao ha.</summary>
    public string? SecondActionText { get; }

    /// <summary>Comando da segunda acao; nulo quando nao ha.</summary>
    public ICommand? SecondActionCommand { get; }

    public bool HasSecondAction => SecondActionCommand is not null;

    /// <summary>Comando sempre executavel que roda a acao da notificacao.</summary>
    private sealed class NotificationActionCommand(Action action) : ICommand
    {
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => action();
    }
}
