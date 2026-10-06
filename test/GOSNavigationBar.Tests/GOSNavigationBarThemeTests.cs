using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.VisualTree;
using FluentAssertions;
// Alias: dentro do namespace GOSNavigationBar.Tests o nome simples GOSNavigationBar resolve para o namespace, nao para o controle.
using NavigationBar = GOSAvaloniaControls.GOSNavigationBar;

namespace GOSNavigationBar.Tests;

/// <summary>
/// A seta entre a legenda e a lista de filhos so aparece quando ha legenda. O defeito que isto fixa: o template ligava o
/// <c>IsVisible</c> da seta a <c>#PART_captionchildren</c>, nome que nao existe (o elemento e <c>PART_CaptionChildren</c>),
/// o binding falhava em silencio e a seta ficava sempre visivel.
/// <para>Carrega o tema pelo mesmo <c>avares://</c> do <c>App.axaml</c> do Nimloth e aplica o template direto: a aplicacao
/// de teste nao tem tema, e a seta e o unico <c>ContentControl</c> do template.</para>
/// </summary>
public class GOSNavigationBarThemeTests
{
    private static (TextBlock caption, ContentControl arrow) ApplyTheme()
    {
        var bar = new NavigationBar();
        bar.Styles.Add(new StyleInclude(new Uri("avares://GOSNavigationBar.Tests/"))
        {
            Source = new Uri("avares://GOSNavigationBar/GOSNavigationBarTheme.axaml")
        });
        var window = new Window { Content = bar };
        window.Show();
        bar.ApplyTemplate();

        var descendants = bar.GetVisualDescendants().ToList();
        return (descendants.OfType<TextBlock>().Single(t => t.Name == "PART_CaptionChildren"),
                descendants.OfType<ContentControl>().Single(c => c is not Button));
    }

    [AvaloniaFact]
    public void SemLegenda_ASetaFicaEscondida()
    {
        var (caption, arrow) = ApplyTheme();

        caption.Text = string.Empty;

        arrow.IsVisible.Should().BeFalse();
    }

    [AvaloniaFact]
    public void ComLegenda_ASetaAparece_ESomeQuandoALegendaSeApaga()
    {
        var (caption, arrow) = ApplyTheme();

        caption.Text = "Phases";
        arrow.IsVisible.Should().BeTrue();

        caption.Text = null;
        arrow.IsVisible.Should().BeFalse();
    }
}
