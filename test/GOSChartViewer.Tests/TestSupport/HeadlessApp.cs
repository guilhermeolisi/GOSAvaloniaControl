using Avalonia;
using Avalonia.Headless;
using Avalonia.Markup.Xaml.Styling;
using GOSAvaloniaControls;
using GOSChartViewer.Tests.TestSupport;

[assembly: AvaloniaTestApplication(typeof(HeadlessApp))]

// Paralelismo desligado no assembly: o Avalonia.Headless isola cada [AvaloniaFact] com estado GLOBAL do processo
// (AvaloniaLocator.EnterScope e Dispatcher.ResetBeforeUnitTests), como explica Nimloth.Views.Tests/TestParallelization.cs.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace GOSChartViewer.Tests.TestSupport;

/// <summary>
/// Aplicacao headless com o tema dos graficos: sem a ControlTheme do <see cref="GOSPieChart"/> o template nao acha o
/// PART_Chart, o grafico interno fica nulo e o ChangeData volta cedo, e o teste passaria sem testar nada.
/// </summary>
public class HeadlessApp : Application
{
    // Como o App.axaml do Nimloth: o .axaml nao tem x:Class, entao entra pelo StyleInclude e nao pelo construtor.
    public override void Initialize() =>
        Styles.Add(new StyleInclude(new Uri("avares://GOSChartViewer.Tests/"))
        {
            Source = new Uri("avares://GOSChartViewer/GOSChartViewerTheme.axaml"),
        });

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<HeadlessApp>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
