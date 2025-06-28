using SkiaSharp;

namespace GOSAvaloniaControls;

public class LiveLegendDark : LiveLegendBase
{
    public LiveLegendDark()
    {
    }

    public LiveLegendDark(bool isVertical) : base(isVertical)
    {
    }

    //protected override SolidColorPaint _backgroundPaint => new(new SKColor(28, 49, 58)) { ZIndex = s_zIndex };
    //protected override SolidColorPaint _fontPaint => new(SKColors.White /*new SKColor(230, 230, 230)*/) { ZIndex = s_zIndex + 1 };
    protected override SKColor _fontPaint => SKColors.White;
}
