using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace GOSAvaloniaControls;

internal class LiveLegendDark : LiveLegendBase
{
    //protected override SolidColorPaint _backgroundPaint => new(new SKColor(28, 49, 58)) { ZIndex = s_zIndex };
    protected override SolidColorPaint _fontPaint => new(SKColors.White /*new SKColor(230, 230, 230)*/) { ZIndex = s_zIndex + 1 };
}
