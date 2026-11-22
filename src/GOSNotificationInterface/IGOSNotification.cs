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

    /// <summary>
    /// Notificacao com duas acoes (E183, lei 6.7 do NIMLOTH-UX.md): o balao mostra so a primeira, e a lista do sino as
    /// duas, como "Copy details" e "Open log folder" numa falha de run. A implementacao padrao descarta a segunda: quem
    /// implementa a interface e mostra botoes sobrescreve este membro.
    /// </summary>
    void AddNotification(byte severity, string message, bool showBallon, string actionText, Action action,
        string secondActionText, Action secondAction)
        => AddNotification(severity, message, showBallon, actionText, action);

    void SetUIContext(SynchronizationContext? uiContext);
}
