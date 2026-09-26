using Avalonia;
using Avalonia.Headless;
using GOSCustomControl.Tests.TestSupport;

[assembly: AvaloniaTestApplication(typeof(HeadlessApp))]

// Paralelismo desligado no assembly: o Avalonia.Headless isola cada [AvaloniaFact] com estado GLOBAL do processo
// (AvaloniaLocator.EnterScope e Dispatcher.ResetBeforeUnitTests), como explica Nimloth.Views.Tests/TestParallelization.cs.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace GOSCustomControl.Tests.TestSupport;

/// <summary>Aplicacao headless minima, sem tema: os testes daqui medem layout, entao o desenho falso basta (sem Skia).</summary>
public class HeadlessApp : Application
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<HeadlessApp>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
