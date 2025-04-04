using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;

namespace GOSAvaloniaControls;

public class LiveLegendBase : SKDefaultLegend
{
    //protected virtual SolidColorPaint _fontPaint => new(SKColors.White)
    //{
    //    //SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
    //    ZIndex = s_zIndex + 1
    //};
    protected virtual SKColor _fontPaint => SKColors.White;
    public bool IsVertical { get; set; } = false;
    protected override Layout<SkiaSharpDrawingContext> GetLayout(Chart chart)
    {
        var stackLayout = new StackLayout
        {
            Orientation = IsVertical ? ContainerOrientation.Vertical : ContainerOrientation.Horizontal,
            Padding = new Padding(15, 4),
            HorizontalAlignment = Align.Start,
            VerticalAlignment = Align.Middle,
        };

        foreach (var series in chart.Series.Where(x => x.IsVisibleAtLegend))
        {
            stackLayout.Children.Add(new LegendItem(series, _fontPaint));
        }

        return stackLayout;
    }
}
internal class LegendItem : StackLayout
{
    public LegendItem(ISeries series, SKColor fontColor)
    {
        Orientation = ContainerOrientation.Horizontal;
        Padding = new Padding(12, 6);
        VerticalAlignment = Align.Middle;
        HorizontalAlignment = Align.Middle;
        Opacity = series.IsVisible ? 1 : 0.5f;

        var miniature = (IDrawnElement<SkiaSharpDrawingContext>)series.GetMiniatureGeometry(null);
        if (miniature is BoundedDrawnGeometry bounded)
        {
            //bounded.Height = 40;
            if (miniature.Stroke?.StrokeThickness < 2)
            {
                miniature.Stroke.StrokeThickness = 2;
            }
        }
        else
        {
            if (miniature.Stroke?.StrokeThickness < 4)
            {
                miniature.Stroke.StrokeThickness = 4;
            }
        }

        Children = [
            miniature,
            new LabelGeometry
            {
                Text = series.Name ?? "?",
                TextSize = 15,
                //Paint = new SolidColorPaint(new SKColor(30, 30, 30)),
                Paint = new SolidColorPaint(fontColor),
                //Padding = new Padding(8, 2, 0, 2),
                Padding = new Padding(8,0,0,0),
                VerticalAlign = Align.Start,
                HorizontalAlign = Align.Start
            }
        ];
    }
}