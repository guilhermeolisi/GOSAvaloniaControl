using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace GOSAvaloniaControls;

internal class LiveLegendBaseBak : IChartLegend
{
    protected static readonly int s_zIndex = 10050;
    private readonly StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext> _stackPanel = new();
    protected virtual SolidColorPaint _fontPaint => new(SKColors.White)
    {
        //SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
        ZIndex = s_zIndex + 1
    };

    public void Draw(Chart chart)
    {
        var legendPosition = chart.GetLegendPosition();

        _stackPanel.X = legendPosition.X;
        _stackPanel.Y = legendPosition.Y;

        chart.AddVisual(_stackPanel);
        if (chart.LegendPosition == LegendPosition.Hidden) chart.RemoveVisual(_stackPanel);
    }

    public void Hide(Chart chart)
    {
        //if (_drawnTask is not null)
        //{
        //    chart.Canvas.RemovePaintTask(_drawnTask);
        //    _drawnTask = null;
        //}
    }

    public LvcSize Measure(Chart chart)
    {
        //_stackPanel.Orientation = ContainerOrientation.Horizontal;
        _stackPanel.Orientation = chart.LegendPosition switch
        {
            LegendPosition.Left or LegendPosition.Right => ContainerOrientation.Vertical,
            LegendPosition.Top or LegendPosition.Bottom => ContainerOrientation.Horizontal,
            _ => ContainerOrientation.Horizontal
        };
        _stackPanel.HorizontalAlignment = chart.LegendPosition switch
        {
            LegendPosition.Left or LegendPosition.Right => Align.Start,
            _ => Align.Middle,
        };
        _stackPanel.MaxWidth = double.MaxValue;
        _stackPanel.MaxHeight = chart.ControlSize.Height;

        // clear the previous elements.
        foreach (var visual in _stackPanel.Children.ToArray())
        {
            _ = _stackPanel.Children.Remove(visual);
            chart.RemoveVisual(visual);
        }


        //var theme = LiveCharts.DefaultSettings.GetTheme<SkiaSharpDrawingContext>();
        var theme = LiveCharts.DefaultSettings.GetTheme();

        foreach (var series in chart.Series.Where(x => x.IsVisibleAtLegend))
        {
            ScatterSeries<ObservablePoint>? scatter = series as ScatterSeries<ObservablePoint>;
            LineSeries<ObservablePoint>? line = series as LineSeries<ObservablePoint>;

            SolidColorPaint StrokeTemp = (SolidColorPaint)(scatter?.Stroke ?? line?.Stroke ?? new SolidColorPaint(theme.GetSeriesColor(series).AsSKColor()));
            //StrokeTemp.ZIndex = s_zIndex + 1;
            //LiveChartsCore.Drawing.IPaint<LiveChartsCore.SkiaSharpView.Drawing.SkiaSharpDrawingContext>

#if DEBUG

#endif

            var panel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
            {
                Padding = new Padding(12, 6),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = chart.LegendPosition switch
                {
                    LegendPosition.Left or LegendPosition.Right => Align.Start,
                    _ => Align.Middle
                },
                //Align.Middle,
                Orientation = ContainerOrientation.Horizontal,

                Children =
                {
                    new SVGVisual
                    {
                        Path = scatter is not null ? SKPath.ParseSvgPathData(SVGPoints.Circle) : SKPath.ParseSvgPathData(SVGPoints.Circle),
                        Width = 15,
                        Height = 15,
                        ClippingMode = ClipMode.None, // required on legends // mark
                        Fill = new SolidColorPaint(StrokeTemp.Color)
                        {
                            ZIndex = s_zIndex + 1
                        },
                        //Fill = new SolidColorPaint(theme.GetSeriesColor(series).AsSKColor())
                        //{
                        //    ZIndex = s_zIndex + 1
                        //},
                        //Stroke = new SolidColorPaint(StrokeTemp.Color)
                        //{
                        //    StrokeThickness = 3, //StrokeTemp.StrokeThickness,
                        //    ZIndex = s_zIndex + 1,
                        //},
                    },
                    new LabelVisual
                    {
                        Text = series.Name ?? string.Empty,
                        Paint = _fontPaint,
                        TextSize = 15,
                        ClippingMode = ClipMode.None, // required on legends // mark
                        Padding = new Padding(8, 0, 0, 0),
                        VerticalAlignment = Align.Start,
                        HorizontalAlignment = Align.Start
                    }
                }
            };

            //panel.PointerDown += GetPointerDownHandler(series);
            _stackPanel.Children.Add(panel);
        }

        return _stackPanel.Measure(chart);
    }

    //private static VisualElementHandler GetPointerDownHandler(IChartSeries series)
    //{
    //    return (visual, args) =>
    //    {
    //        series.IsVisible = !series.IsVisible;
    //    };
    //}
}
