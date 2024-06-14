using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace GOSAvaloniaControls;

internal class LiveLegendLigth : LiveLegendBase
{
    //protected override SolidColorPaint _backgroundPaint => new(new SKColor(28, 49, 58)) { ZIndex = s_zIndex };
    protected override SolidColorPaint _fontPaint => new(SKColors.Black) { ZIndex = s_zIndex + 1 };
}
