namespace GOSAvaloniaControls;

public interface IGOSNotification
{
    bool UIContextIsNull { get; }

    void AddNotification(byte severity, string message, bool showBallon);

    /// <summary>
    /// Notificacao com uma acao (E175), como "Open results" no fim de um run. A implementacao padrao descarta a acao e
    /// mostra so a mensagem: quem implementa a interface e mostra botoes sobrescreve este membro.
    /// </summary>
    void AddNotification(byte severity, string message, bool showBallon, string actionText, Action action)
        => AddNotification(severity, message, showBallon);

    void SetUIContext(SynchronizationContext? uiContext);
}
