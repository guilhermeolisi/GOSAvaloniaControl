using Avalonia.Markup.Xaml;
using AvaloniaStyles = Avalonia.Styling.Styles;

namespace GOSAvaloniaControls;

public class GOSTextEditorTheme : AvaloniaStyles
{
    public GOSTextEditorTheme() => AvaloniaXamlLoader.Load(this);
}
