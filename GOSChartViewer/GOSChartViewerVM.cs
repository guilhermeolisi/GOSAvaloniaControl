using Avalonia.Threading;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace GOSAvaloniaControls;

public partial class GOSChartViewer
{
    static SKColor[] DiffractogramColorsLigth = { new(100, 100, 100), new(80, 161, 79), new(228, 86, 74), new(193, 132, 3), new(0, 132, 188), new(166, 38, 164), new(8, 151, 179) };
    static SKColor[] DiffractogramColorsDark = { new(154, 154, 154), new(152, 195, 121), new(224, 108, 117), new(229, 192, 123), new(97, 175, 240), new(198, 120, 221), new(86, 182, 194) };

    private Dispatcher UIDispatcher = Dispatcher.UIThread;

    private ObservableCollection<string>? ExperimentalsLabel { get; set; }
    private ObservableCollection<ObservablePoint?> DataPoints = new();

    int SelectedIndex { get; set; }

    private readonly DrawMarginFrame DrawMarginFrame = new() { Fill = null };

    private readonly ObservableCollection<ISeries> Series = new()
    {
        new LineSeries<ObservablePoint>
        {
            Name = "Experimental",
            GeometryFill = null,
            GeometryStroke = null,
            Fill = null,
            LineSmoothness = 0,
            //TooltipLabelFormatter = (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue}, {chartPoint.SecondaryValue:F4}"
            DataPadding = new LvcPoint(0.01, 0.01),
        },
    };
    private Axis[][] Axes =
    [
        [
            new Axis
            {
                Position = AxisPosition.Start,
                Name = "X",
                LabelsPaint = null,
            },
        ],
        [
            new Axis
            {
                Position = AxisPosition.Start,
                Name = "Y",
                LabelsPaint = null,
            },
        ]
    ];
    private void ChangeTheme()
    {
        DrawMarginFrame.Stroke = new SolidColorPaint { Color = IsDarkTheme ? SKColors.White : SKColors.Black };
        for (int i = 0; i < Axes.Length; i++)
        {
            for (int j = 0; j < Axes[i].Length; j++)
            {
                Axes[i][j].NamePaint = new SolidColorPaint { Color = IsDarkTheme ? SKColors.White : SKColors.Black };
                Axes[i][j].LabelsPaint = new SolidColorPaint { Color = IsDarkTheme ? SKColors.White : SKColors.Black };
                Axes[i][j].SeparatorsPaint = new SolidColorPaint { Color = IsDarkTheme ? new SKColor(100, 100, 100, 255) /*SKColors.DarkGray*/ : new SKColor(210, 210, 210, 255) /*SKColors.LightGray*/ };
            }
        }

        if (Series is null)
            return;
        int indSeries = 0;
        int k = 0;
        foreach (var item in Series)
        {
            if (k >= DiffractogramColorsDark.Length)
            {
                k = 0;
            }
            if (item is ScatterSeries<ObservablePoint> scat2)
            {
                scat2.Stroke = new SolidColorPaint(IsDarkTheme ? DiffractogramColorsDark[k] : DiffractogramColorsLigth[k]) { StrokeThickness = 0 };
            }
            else if (item is LineSeries<ObservablePoint> line2)
            {
                line2.Stroke = new SolidColorPaint(IsDarkTheme ? DiffractogramColorsDark[k] : DiffractogramColorsLigth[k]) { StrokeThickness = 2 };
            }

            k++;
            indSeries++;
        }
    }
    //bool isChangingData;
    public void SetData(List<string>? experimentals)
    {
        //isChangingData = true;
        if (experimentals is null || experimentals.Count == 0 || experimentals.Count == 1)
        {
            (ExperimentalsLabel ??= new()).Clear();
        }
        else
        {
            for (int i = 0; i < experimentals?.Count; i++)
            {
                string temp = experimentals[i] is not null ? experimentals[i] : "Experimental " + (i + 1);
                if (i > (ExperimentalsLabel ??= new()).Count - 1)
                {
                    ExperimentalsLabel.Add(temp);
                }
                else if (ExperimentalsLabel[i] != temp)
                {
                    ExperimentalsLabel[i] = temp;
                }
            }
            while ((ExperimentalsLabel ??= new())?.Count > experimentals?.Count)
            {
                ExperimentalsLabel.RemoveAt(ExperimentalsLabel.Count - 1);
            }
            if (ExperimentalsLabel?.Count > 0 && SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }
        }
        //isChangingData = false;
        ProcessPoints();
    }
    private void ProcessPoints()
    {
        //1 Verifica os dados
        if (Data is null || Data.Count == 0)
        {
            ClearDatas();
            return;
        }

        int expIndex = ExperimentalsLabel is null || ExperimentalsLabel.Count == 0 || ExperimentalsLabel.Count == 1 ? 0 : SelectedIndex >= 0 ? SelectedIndex : 0;

        ChangeDataToObservableCollection(Data, DataPoints);

        if (IsZooming) //Se for full o limite é definido pelo zoom
            ResetZoom();
    }
    protected void ResetZoom()
    {
        if (Axes is null || Axes.Length == 0)
            return;
        Axes[0][0].MinLimit = null;
        Axes[0][0].MaxLimit = null;
    }
    private void Data_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null && e.NewItems.Count == 1)
                {
                    if (e.NewStartingIndex == Data?.Count - 1)
                    {
                        DataPoints.Add(new ObservablePoint((((double X, double Y))e.NewItems[0]).X, (((double X, double Y))e.NewItems[0]).Y));
                    }
                    else
                    {
                        DataPoints.Insert(e.NewStartingIndex, new((((double X, double Y))e.NewItems[0]).X, (((double X, double Y))e.NewItems[0]).Y));
                    }
                }
                else
                {

                }
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                DataPoints.RemoveAt(e.OldStartingIndex);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                break;

        }
    }

    private void ChangeDataToObservableCollection(ObservableCollection<(double X, double Y)> data, ObservableCollection<ObservablePoint> obs)
    {
        obs.Clear();
        int indPlus = 0;
        for (int i = 0; i < data.Count; i++)
        {
            obs.Add(new ObservablePoint(data[i].X, data[i].Y));

        }
#if DEBUG
        if (obs == Series[0].Values)
        {

        }
#endif
    }
    private void ClearDatas()
    {
        for (int i = 0; i < Series?.Count; i++)
        {
            (Series[i].Values as ObservableCollection<ObservablePoint>).Clear();
        }
    }
}
