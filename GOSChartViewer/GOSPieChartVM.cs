using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace GOSAvaloniaControls;

public partial class GOSPieChart
{
    static SKColor[] ChartColorsLigth = [new(80, 161, 79), new(228, 86, 74), new(193, 132, 3), new(0, 132, 188), new(166, 38, 164), new(8, 151, 179)];
    static SKColor[] ChartColorsDark = [new(152, 195, 121), new(224, 108, 117), new(229, 192, 123), new(97, 175, 240), new(198, 120, 221), new(86, 182, 194)];

    private readonly ObservableCollection<PieSeries<double>> DataToShow = new();

    private void ChangeTheme()
    {


        int indSeries = 0;
        int k = 0;
        foreach (var item in DataToShow)
        {
            if (k >= ChartColorsDark.Length)
            {
                k = 0;
            }
            if (item is PieSeries<double> pie)
            {
                pie.Stroke = new SolidColorPaint(IsDarkTheme ? ChartColorsDark[k] : ChartColorsLigth[k]) { StrokeThickness = 0 };
            }

            k++;
            indSeries++;
        }
        if (_chart is null)
            return;
        _chart.Legend = IsDarkTheme ? new LiveLegendDark() : new LiveLegendLigth();

        //Forçar o Update
        _chart.CoreChart.Update(new LiveChartsCore.Kernel.ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
    }
    //bool isChangingData;
    public void SetData()
    {

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

        ChangeDataToObservableCollection(Data, DataToShow);
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
                        DataToShow.Add(new PieSeries<double>() { Values = [(double)e.NewItems[0]] });
                    }
                    else
                    {
                        DataToShow.Insert(e.NewStartingIndex, new() { Values = [(double)e.NewItems[0]] });
                    }
                }
                else
                {

                }
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                DataToShow.RemoveAt(e.OldStartingIndex);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                break;

        }
    }

    private void ChangeDataToObservableCollection(ObservableCollection<double>? data, ObservableCollection<PieSeries<double>> obs)
    {
        if (data is null)
        {
            obs.Clear();
            return;
        }
        obs.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            obs.Add(new PieSeries<double>()
            {
                Values = [data[i]],
                Name = Labels is null || i >= Labels.Count ? string.Empty : Labels[i],
                ToolTipLabelFormatter =
                    point =>
                    {
                        var pv = point.Coordinate.PrimaryValue;
                        var sv = point.StackedValue!;

                        var a = (ShowValueToolTip ? pv.ToString(string.IsNullOrEmpty(StringFormatValue) ? string.Empty : StringFormatValue) : string.Empty)
                            + (ShowValueToolTip && ShowPercentToolTip ? "(" : string.Empty)
                            + (ShowPercentToolTip ? sv.Share.ToString("P2") : string.Empty)
                            + (ShowValueToolTip && ShowPercentToolTip ? ")" : string.Empty);
                        return a;
                    }
            });
        }
    }
    private void ClearDatas()
    {
        DataToShow.Clear();
    }
}
