using SkiaSharp;

namespace GOSAvaloniaControls;

public class LiveLegendLigth : LiveLegendBase
{
    public LiveLegendLigth()
    {
    }

    public LiveLegendLigth(bool isVertical) : base(isVertical)
    {
    }

    //protected override SolidColorPaint _backgroundPaint => new(new SKColor(28, 49, 58)) { ZIndex = s_zIndex };
    //protected override SolidColorPaint _fontPaint => new(SKColors.Black) { ZIndex = s_zIndex + 1 };
    protected override SKColor _fontPaint => SKColors.Black;
}
