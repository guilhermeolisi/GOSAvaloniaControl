using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAssertions;
using GOSAvaloniaControls;

namespace GOSNotification.Tests;

/// <summary>
/// O balao de notificacao (E108, Parte 4): ele aparece sem tirar o foco do teclado de quem estava escrevendo, e sem
/// tomar para si o clique seguinte na janela.
/// </summary>
public class GOSNotificationControlTests
{
    private sealed record Cenario(Window Janela, TextBox Editor, Button Outro, GOSNotificationControl Sino, FlyoutBase Balao);

    private static Cenario Montar()
    {
        var editor = new TextBox { Width = 300, Height = 40, AcceptsReturn = false };
        var outro = new Button { Content = "Other", Width = 200, Height = 60 };
        var sino = new GOSNotificationControl { SizeBell = 30 };
        var painel = new StackPanel { Spacing = 10, Margin = new Thickness(20) };
        painel.Children.Add(editor);
        painel.Children.Add(outro);
        painel.Children.Add(sino);
        var janela = new Window { Width = 800, Height = 600, Content = painel };
        janela.Show();
        Dispatcher.UIThread.RunJobs();

        var botaoSino = sino.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PART_buttonBell");
        var balao = FlyoutBase.GetAttachedFlyout(botaoSino)!;
        editor.Focus();
        Dispatcher.UIThread.RunJobs();
        return new Cenario(janela, editor, outro, sino, balao);
    }

    private static void Notificar(Cenario c)
    {
        c.Sino.AddNotification(1, "Refinement finished", true);
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void AddNotification_ComBalao_AbreOBalao()
    {
        var c = Montar();

        Notificar(c);

        c.Balao.IsOpen.Should().BeTrue("a notificacao com balao abre o balao");
    }

    [AvaloniaFact]
    public void AddNotification_ComBalao_MantemOFocoDoTecladoNoEditor()
    {
        var c = Montar();
        c.Editor.IsFocused.Should().BeTrue("o cenario comeca com o foco no editor");

        Notificar(c);

        c.Janela.FocusManager!.GetFocusedElement().Should().BeSameAs(c.Editor);
        c.Editor.IsKeyboardFocusWithin.Should().BeTrue();
        c.Janela.IsActive.Should().BeTrue("a janela principal continua ativa");
    }

    [AvaloniaFact]
    public void AddNotification_ComBalao_DigitarDepoisVaiParaOEditor()
    {
        var c = Montar();

        Notificar(c);
        c.Janela.KeyTextInput("abc");
        Dispatcher.UIThread.RunJobs();

        c.Editor.Text.Should().Be("abc");
    }

    [AvaloniaFact]
    public void BalaoAberto_CliqueNoutroControleDaJanela_ChegaAoControle()
    {
        var c = Montar();
        int cliques = 0;
        c.Outro.Click += (_, _) => cliques++;
        Notificar(c);
        c.Balao.IsOpen.Should().BeTrue();

        var centro = c.Outro.TranslatePoint(new Point(c.Outro.Bounds.Width / 2, c.Outro.Bounds.Height / 2), c.Janela)!.Value;
        c.Janela.MouseDown(centro, MouseButton.Left);
        c.Janela.MouseUp(centro, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();

        cliques.Should().Be(1, "o balao aberto nao pode engolir o clique seguinte na janela");
    }

    [AvaloniaFact]
    public void BalaoAberto_CliqueNoEditor_LevaOFocoAoEditorEDigitarFunciona()
    {
        var c = Montar();
        c.Outro.Focus();
        Dispatcher.UIThread.RunJobs();
        Notificar(c);

        var centro = c.Editor.TranslatePoint(new Point(10, c.Editor.Bounds.Height / 2), c.Janela)!.Value;
        c.Janela.MouseDown(centro, MouseButton.Left);
        c.Janela.MouseUp(centro, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
        c.Janela.KeyTextInput("x");
        Dispatcher.UIThread.RunJobs();

        c.Editor.IsFocused.Should().BeTrue();
        c.Editor.Text.Should().Be("x");
    }

    // E175: notificacao com acao ("Open results" no fim de um run). Antes a acao nao existia; a implementacao padrao
    // da interface a descarta, entao o controle tem de sobrescreve-la e mostrar o botao.

    [AvaloniaFact]
    public void AddNotificationComAcao_PelaInterface_GuardaTextoEComandoQueRodaAAcao()
    {
        var c = Montar();
        int chamadas = 0;

        ((IGOSNotification)c.Sino).AddNotification(1, "Walk finished", true, "Open results", () => chamadas++);
        Dispatcher.UIThread.RunJobs();

        // O valor padrao de Items e uma colecao compartilhada entre instancias: os outros testes deixam itens nela.
        var item = c.Sino.Items.Last();
        item.Message.Should().Be("Walk finished");
        item.ActionText.Should().Be("Open results");
        item.HasAction.Should().BeTrue();
        item.ActionCommand!.Execute(null);
        chamadas.Should().Be(1);
    }

    [AvaloniaFact]
    public void AddNotificationComAcao_BotaoDoBalaoRodaAAcao()
    {
        var c = Montar();
        int chamadas = 0;

        c.Sino.AddNotification(1, "Walk finished", true, "Open results", () => chamadas++);
        Dispatcher.UIThread.RunJobs();

        var conteudo = (Control)((Flyout)c.Balao).Content!;
        var botao = conteudo.GetVisualDescendants().OfType<Button>()
            .Last(b => b.Classes.Contains("notificationAction") && b.IsVisible);
        botao.IsVisible.Should().BeTrue();
        botao.Content.Should().Be("Open results");
        botao.Command!.Execute(null);
        chamadas.Should().Be(1);
    }

    [AvaloniaFact]
    public void ListaDoSino_CliqueNoBotaoDeAcao_NaoRemoveOItem_CliqueNoXRemove()
    {
        var c = Montar();
        c.Sino.AddNotification(1, "Walk finished", false, "Open results", () => { });
        c.Sino.AddNotification(1, "Plain message", false);
        Dispatcher.UIThread.RunJobs();
        var comAcao = c.Sino.Items[^2];
        var semAcao = c.Sino.Items[^1];
        int antes = c.Sino.Items.Count;
        var botaoSino = c.Sino.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PART_buttonBell");
        var lista = (Flyout)botaoSino.Flyout!;
        lista.ShowAt(botaoSino);
        Dispatcher.UIThread.RunJobs();
        var botoes = ((Control)lista.Content!).GetVisualDescendants().OfType<Button>().ToList();

        var acao = botoes.Single(b => b.Classes.Contains("notificationAction") && b.DataContext == comAcao);
        acao.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();
        c.Sino.Items.Should().HaveCount(antes, "o botao de acao nao e o X de remover").And.Contain(comAcao);

        var remover = botoes.Single(b => b.Classes.Contains("removeNotification") && b.DataContext == semAcao);
        remover.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();
        c.Sino.Items.Should().HaveCount(antes - 1).And.NotContain(semAcao);
    }
}
