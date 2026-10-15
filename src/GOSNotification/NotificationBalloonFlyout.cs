using Avalonia.Controls;
using System.ComponentModel;

namespace GOSAvaloniaControls;

/// <summary>
/// Flyout do balao de notificacao: aparece por cima da janela sem tomar o foco nem a entrada de ninguem (E108, Parte 4).
/// <para>O <see cref="Flyout"/> padrao cria o <c>Popup</c> com <c>IsLightDismissEnabled = true</c>. Com isso, enquanto o
/// balao esta aberto, a camada de light dismiss cobre a janela inteira e o primeiro clique em qualquer controle so fecha o
/// balao, sem chegar ao controle: o usuario clica no editor para continuar escrevendo e o foco nao vai para la. O mesmo
/// light dismiss fecha o balao quando a janela perde o foco. E o <c>Popup</c> chama <c>TakeFocus</c> do host ao abrir
/// (<c>TakesFocusFromNativeControl</c>), que no Win32 devolve o foco nativo a janela dona.</para>
/// <para>Aqui os dois ficam desligados antes de cada abertura. O balao some pelo timer do <see cref="GOSNotificationControl"/>.</para>
/// </summary>
public class NotificationBalloonFlyout : Flyout
{
    protected override void OnOpening(CancelEventArgs args)
    {
        Popup.IsLightDismissEnabled = false;
        Popup.TakesFocusFromNativeControl = false;
        base.OnOpening(args);
    }
}
