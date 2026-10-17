using Avalonia;
using Avalonia.Headless;
using Avalonia.Markup.Xaml.Styling;
using FluentAvalonia.Styling;
using GOSNotification.Tests.TestSupport;

[assembly: AvaloniaTestApplication(typeof(HeadlessApp))]

// Paralelismo desligado no assembly: o Avalonia.Headless isola cada [AvaloniaFact] com estado GLOBAL do processo
// (AvaloniaLocator.EnterScope e Dispatcher.ResetBeforeUnitTests), como explica Nimloth.Views.Tests/TestParallelization.cs.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace GOSNotification.Tests.TestSupport;

/// <summary>
/// Aplicacao headless com o tema que o template do sino precisa, na ordem do App.axaml do Nimloth: o FluentAvaloniaTheme
/// (InfoBar, InfoBadge, Window com a camada de light dismiss) e depois o tema do proprio controle.
/// </summary>
public class HeadlessApp : Application
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<HeadlessApp>()
            .UseSkia()
            .UseHarfBuzz()
            .WithInterFont()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });

    public override void Initialize()
    {
        Styles.Add(new FluentAvaloniaTheme());
        // Avalonia 12: o avares:// de outro assembly so acha o XAML compilado se o assembly ja estiver carregado.
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(GOSAvaloniaControls.GOSNotificationControl).TypeHandle);
        var baseUri = new Uri("avares://GOSNotification.Tests/");
        Styles.Add(new StyleInclude(baseUri) { Source = new Uri("avares://GOSNotification/GOSNotificationControlTheme.axaml") });
    }
}
