using Avalonia.Markup.Xaml;
using AvaloniaStyles = Avalonia.Styling.Styles;

namespace GOSAvaloniaControls;

public class GOSChartViewerTheme : AvaloniaStyles
{
    public GOSChartViewerTheme() => AvaloniaXamlLoader.Load(this);
}
