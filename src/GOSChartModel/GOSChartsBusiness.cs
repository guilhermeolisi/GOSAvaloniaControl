using BaseLibrary;
using BaseLibrary.Numbers;
using GOSAvaloniaControls;
using GOSAvaloniaServices;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Collections;
using System.Reflection.Emit;

namespace GOSAvaloniaControls;

public class GOSChartsBusiness : IGOSChartsBusiness
{
    static IColorServices colorServices;
    static IFileServices fileServices;

    static DrawMarginFrame DrawMarginFrame;
    static LiveLegendBase _legendBase;
    static double darkLightness, lightLightness;
    static GOSChartsBusiness()
    {

        DrawMarginFrame = new DrawMarginFrame
        {
            Fill = null,
            Stroke = new SolidColorPaint { Color = SKColors.Black },
        };

        _legendBase = new LiveLegendLigth();
        lightLightness = 0.44;
        darkLightness = 0.66;
    }
    public GOSChartsBusiness(IColorServices? colorServices = null, IFileServices? fileServices = null)
    {
        GOSChartsBusiness.colorServices = colorServices ?? ContainersDIServices.Resolve<IColorServices>() ?? new ColorServices();
        GOSChartsBusiness.fileServices = fileServices ?? ContainersDIServices.Resolve<IFileServices>() ?? new FileServices();
    }
    public void SaveToTextCartesianChart(IEnumerable<ISeries> mainSeries, IEnumerable<ISeries>? stackDownSeries, string title, string labelX, string filePathToSave)
    {
        // Selecionar as séries que tem os mesmos valroes de X que a primeira série
        
    }
    public void SaveToImageCartesianChart(IEnumerable<ISeries> mainSeries, IEnumerable<ISeries>? stackDownSeries, bool needLigth, string title, string xLabel, string yLabel, string filePathToSave, FormatImage format, LegendPosition legendPosition, int width, int height, double? xmin, double? xmax, double? ymin, double? ymax)
    {
        List<ISeries> seriesTemp = [];

        double maxY, minY, tolerance;
        tolerance = double.NaN;
        maxY = double.MinValue; minY = double.MaxValue;
        foreach (var item in mainSeries)
        {
            if (!item.IsVisible)
                continue;

            ISeries newSeries;
            if (needLigth)
            {
                newSeries = CopyISerie(item, needLigth, 0);
            }
            else
            {
                newSeries = item;
            }
            seriesTemp.Add(newSeries);

            (double tempMinY, double tempMaxY) = GetMinMaxSerie(newSeries);
            if (tempMaxY > maxY)
                maxY = tempMaxY;
            if (tempMinY < minY)
                minY = tempMinY;
        }

        tolerance = (maxY - minY) * 0.01;

        if (stackDownSeries is not null)
        {
            double correctStackDown = minY;
            foreach (var item in stackDownSeries)
            {
                if (!item.IsVisible)
                    continue;
                ISeries newSeries;
                if (needLigth)
                {
                    newSeries = CopyISerie(item, needLigth, 0);
                }
                else
                {
                    newSeries = item;
                }
                (_, double temp) = GetMinMaxSerie(newSeries);
                //correctStackDown += (minY - tolerance - temp);
                correctStackDown += -tolerance - temp;
                CorrectYValue(newSeries, correctStackDown);
                seriesTemp.Add(newSeries);
            }
        }

        var chart = CreateCartesianChart(seriesTemp, title, xLabel, yLabel, legendPosition, filePathToSave, format, width, height, xmin, xmax, ymin, ymax);
        SaveChart(chart, filePathToSave, format);
    }
    public void SaveToImagePieChart(IEnumerable<ISeries> series, bool needLigth, string title, string filePathToSave, FormatImage format, LegendPosition legendPosition, int width, int height)
    {

        List<ISeries> seriesTemp = [];
        double total = 0;
        if (series is IEnumerable<PieSeries<double>> pieSeries)
        {
            total = pieSeries.Sum(x => x.Values.Sum());
        }

        foreach (var item in series)
        {
            if (!item.IsVisible)
                continue;
            ISeries newSeries;
            if (needLigth)
            {
                newSeries = CopyISerie(item, needLigth, total);
            }
            else
            {
                newSeries = item;
            }
            seriesTemp.Add(newSeries);
        }

        var chart = CreatePieChart(seriesTemp, legendPosition, title, width, height);
        SaveChart(chart, filePathToSave, format);
    }
    private SolidColorPaint? DarkenPaint(SolidColorPaint? paint)
    {
        if (paint is null)
            return null;
        SKColor color = paint.Color;
        (byte r, byte g, byte b) = colorServices.RGBChangeLightness(color.Red, color.Green, color.Blue, lightLightness);
        return new SolidColorPaint { Color = new SKColor(r, g, b, color.Alpha), StrokeThickness = paint.StrokeThickness };
    }
    public ISeries CopyISerie(ISeries series, bool needLight, double total)
    {
        ISeries newSeries;
        if (series is ScatterSeries<ObservablePoint> sca)
        {
            ScatterSeries<ObservablePoint> sca2 = new ScatterSeries<ObservablePoint>();
            sca2.Values = sca.Values;
            sca2.Name = sca.Name;
            sca2.Fill = sca.Fill;
            sca2.GeometrySize = sca.GeometrySize;
            sca2.DataPadding = sca.DataPadding;
            sca2.YToolTipLabelFormatter = sca.YToolTipLabelFormatter;
            sca2.XToolTipLabelFormatter = sca.XToolTipLabelFormatter;
            sca2.IsVisible = sca.IsVisible;
            if (needLight)
            {
                SolidColorPaint? paint = ((SolidColorPaint?)sca.Stroke)!;
                sca2.Stroke = DarkenPaint(paint);
            }
            else
            {
                sca2.Stroke = sca.Stroke;
            }
            newSeries = sca2;
        }
        else if (series is LineSeries<ObservablePoint> line)
        {
            LineSeries<ObservablePoint> line2 = new LineSeries<ObservablePoint>();
            line2.Values = line.Values;
            line2.Name = line.Name;
            line2.Fill = line.Fill;
            line2.GeometrySize = line.GeometrySize;
            line2.GeometryFill = line.GeometryFill;
            line2.GeometryStroke = line.GeometryStroke;
            line2.LineSmoothness = line.LineSmoothness;
            line2.DataPadding = line.DataPadding;
            line2.YToolTipLabelFormatter = line.YToolTipLabelFormatter;
            line2.XToolTipLabelFormatter = line.XToolTipLabelFormatter;
            line2.IsVisible = line.IsVisible;
            if (needLight)
            {
                SolidColorPaint? paint = ((SolidColorPaint?)line.Stroke)!;
                line2.Stroke = DarkenPaint(paint);
                paint = ((SolidColorPaint?)line.Fill)!;
                line2.Fill = DarkenPaint(paint);
            }
            else
            {
                line2.Stroke = line.Stroke;
                line2.Fill = line.Fill;
            }

            newSeries = line2;
        }
        else if (series is ScatterSeries<ObservablePoint, VariableSVGPathGeometry> scatRef)
        {
            ScatterSeries<ObservablePoint, VariableSVGPathGeometry> scatRef2 = new ScatterSeries<ObservablePoint, VariableSVGPathGeometry>();
            scatRef2.Values = scatRef.Values;
            scatRef2.Name = scatRef.Name;
            scatRef2.Fill = scatRef.Fill;
            scatRef2.GeometrySize = scatRef.GeometrySize;
            scatRef2.DataPadding = scatRef.DataPadding;
            scatRef2.GeometrySvg = scatRef.GeometrySvg;
            scatRef2.YToolTipLabelFormatter = scatRef.YToolTipLabelFormatter;
            scatRef2.XToolTipLabelFormatter = scatRef.XToolTipLabelFormatter;
            scatRef2.IsVisible = scatRef.IsVisible;
            if (needLight)
            {
                SolidColorPaint? paint = ((SolidColorPaint?)scatRef.Stroke)!;
                scatRef2.Stroke = DarkenPaint(paint);
                paint = ((SolidColorPaint?)scatRef.Fill)!;
                scatRef2.Fill = DarkenPaint(paint);
            }
            else
            {
                scatRef2.Stroke = scatRef.Stroke;
                scatRef2.Fill = scatRef.Fill;
            }

            newSeries = scatRef2;
        }
        else if (series is PieSeries<double> pieDouble)
        {
            PieSeries<double> pieDouble2 = new PieSeries<double>();
            pieDouble2.Values = pieDouble.Values;
            pieDouble2.Name = pieDouble.Name + $" ({(pieDouble.Values.Sum() / total * 100):G3}%)";
            pieDouble2.DataPadding = pieDouble.DataPadding;
            pieDouble2.IsVisible = pieDouble.IsVisible;
            if (needLight)
            {
                SolidColorPaint? paint = ((SolidColorPaint?)pieDouble.Fill)!;
                pieDouble2.Fill = DarkenPaint(paint);
                paint = ((SolidColorPaint?)pieDouble.Stroke)!;
                pieDouble2.Stroke = DarkenPaint(paint);
            }
            else
            {
                pieDouble2.Fill = pieDouble.Fill;
                pieDouble2.Stroke = pieDouble.Stroke;
            }
            newSeries = pieDouble2;
        }
        else
        {
            //Não deve entrar aqui
            newSeries = null;
        }
        return newSeries;
    }
    private (double min, double max) GetMinMaxSerie(ISeries series)
    {
        double maxY = double.NaN;
        double minY = double.NaN;
        if (series is ScatterSeries<ObservablePoint> sca)
        {
            maxY = sca.Values.Max(a => a?.Y ?? double.MinValue);
            minY = sca.Values.Min(a => a?.Y ?? double.MaxValue);
        }
        else if (series is LineSeries<ObservablePoint> line)
        {
            maxY = line.Values.Max(a => a?.Y ?? double.MinValue);
            minY = line.Values.Min(a => a?.Y ?? double.MaxValue);
        }
        else if (series is PieSeries<double> pieDouble)
        {
            maxY = pieDouble.Values.Max();
            minY = pieDouble.Values.Min();
        }
        else if (series is ScatterSeries<ObservablePoint, VariableSVGPathGeometry> scatRef)
        {
            maxY = scatRef.Values.Max(a => a?.Y ?? double.MinValue);
            minY = scatRef.Values.Min(a => a?.Y ?? double.MaxValue) - 200;
        }
        return (minY, maxY);
    }
    private void CorrectYValue(ISeries series, double correction)
    {
        if (series is ScatterSeries<ObservablePoint> sca)
        {
           foreach (var item in sca.Values)
            {
                if (item is not null && item.Y.HasValue)
                    item.Y += correction;
            }
        }
        else if (series is LineSeries<ObservablePoint> line)
        {
            foreach (var item in line.Values)
            {
                if (item is not null && item.Y.HasValue)
                    item.Y += correction;
            }
        }
        else if (series is PieSeries<double> pieDouble)
        {
            
        }
        else if (series is ScatterSeries<ObservablePoint, VariableSVGPathGeometry> scatRef)
        {
            foreach (var item in scatRef.Values)
            {
                if (item is not null && item.Y.HasValue)
                    item.Y += correction;
            }
        }
    }
    private InMemorySkiaSharpChart CreateCartesianChart(IEnumerable<ISeries> series, string title, string xLabel, string yLabel, LegendPosition legendPosition, string filePathToSave, FormatImage format, int width, int height, double? xmin, double? xmax, double? ymin, double? ymax)
    {
        InMemorySkiaSharpChart chart = new SKCartesianChart
        {
            Width = width,
            Height = height,
            Background = SKColors.White,
            DrawMarginFrame = DrawMarginFrame,
            AnimationsSpeed = TimeSpan.FromMilliseconds(0),
            EasingFunction = null,
            LegendBackgroundPaint = new SolidColorPaint { Color = SKColors.White },
            Series = series,
            Title = new LabelVisual
            {
                Text = title,
                TextSize = 30,
                Padding = new Padding(15),
                Paint = new SolidColorPaint(0xff303030)
            },
            XAxes =
            [
                new Axis
                {
                    Position = AxisPosition.Start,
                    Name = xLabel,
                    //Labeler = value => value.ToString("G6"),
                    NamePaint = new SolidColorPaint { Color = SKColors.Black },
                    LabelsPaint = new SolidColorPaint { Color = SKColors.Black },
                    SeparatorsPaint = new SolidColorPaint { Color = new SKColor(210, 210, 210, 255) },
                    MinLimit = xmin,
                    MaxLimit = xmax
                }
            ],
            YAxes =
            [
                new Axis
                {
                    Position = AxisPosition.Start,
                    Name = yLabel,
                    //Labeler = value => value.ToString("G6"),
                    NamePaint = new SolidColorPaint { Color = SKColors.Black },
                    LabelsPaint = new SolidColorPaint { Color = SKColors.Black },
                    SeparatorsPaint = new SolidColorPaint { Color = new SKColor(210, 210, 210, 255) },
                    MinLimit = ymin,
                    MaxLimit = ymax
                }
            ]
        };
        (chart as SKCartesianChart).Legend = new LiveLegendLigth(legendPosition == LegendPosition.Right || legendPosition == LegendPosition.Left);
        (chart as SKCartesianChart).LegendPosition = legendPosition;
        return chart;
    }
    private InMemorySkiaSharpChart CreatePieChart(IEnumerable<ISeries> series, LegendPosition legendPosition, string title, int width, int height)
    {
        InMemorySkiaSharpChart chart = new SKPieChart
        {
            Width = width,
            Height = height,
            Background = SKColors.White,
            AnimationsSpeed = TimeSpan.FromMilliseconds(0),
            EasingFunction = null,
            LegendBackgroundPaint = new SolidColorPaint { Color = SKColors.White },
            Series = series,
            Title = new LabelVisual
            {
                Text = title,
                TextSize = 30,
                Padding = new Padding(15),
                Paint = new SolidColorPaint(0xff303030)
            },
        };
        (chart as SKPieChart).Legend = new LiveLegendLigth(legendPosition == LegendPosition.Right || legendPosition == LegendPosition.Left);
        (chart as SKPieChart).LegendPosition = legendPosition;
        return chart;
    }
    private void SaveChart(InMemorySkiaSharpChart chart, string filePathToSave, FormatImage format)
    {
        filePathToSave = fileServices.Name.FileNameAvailable(filePathToSave);
        switch (format)
        {

            case FormatImage.SVG:
                // additionally you can save a chart as svg:
                // for more info see: https://github.com/mono/SkiaSharp/blob/main/tests/Tests/SKCanvasTest.cs#L396
                using (var stream = new MemoryStream())
                {

                    var svgCanvas = SKSvgCanvas.Create(SKRect.Create(chart.Width, chart.Height), stream);
                    chart.DrawOnCanvas(svgCanvas);
                    svgCanvas.Dispose(); // <- dispose it before using the stream, otherwise the svg could not be completed.

                    stream.Position = 0;
                    using (var fs = new FileStream(filePathToSave, FileMode.OpenOrCreate))
                    {
                        stream.CopyTo(fs);
                    }
                }
                break;
            case FormatImage.PNG:
            default:
                // you can save the image to png (by default)
                // or use the second argument to specify another format.
                chart.SaveImage(filePathToSave);
                break;
        }
    }
    private IReadOnlyCollection<ObservablePoint> ObservablePointsFromArray(double[][] x, double[][] y)
    {
        var points = new List<ObservablePoint>();
        for (int i = 0; i < x.Length; i++)
        {
            for (int j = 0; j < x[i].Length; j++)
            {
                points.Add(new ObservablePoint(x[i][j], y[i][j].IsNaN() ? null : y[i][j]));
            }
            if (i < x.Length - 1)
                points.Add(null);
        }
        return points.ToArray();
    }
    private IReadOnlyCollection<ObservablePoint> ObservablePointsFromArray(List<ObservablePoint>[]? ph)
    {
        var points = new List<ObservablePoint>();
        for (int i = 0; i < ph.Length; i++) // Cada arquivo experimental
        {
            for (int j = 0; j < ph[i].Count; j++)
            {
                points.Add(ph[i][j]);
            }
            if (i < ph.Length - 1)
                points.Add(null);
        }
        return points.ToArray();
    }
    public enum FormatImage : byte
    {
        PNG,
        SVG
    }

}
