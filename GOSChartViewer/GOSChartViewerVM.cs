using Avalonia.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSAvaloniaControls;

public partial class GOSChartViewer
{
    static SKColor[] DiffractogramColorsLigth = { new(100, 100, 100), new(80, 161, 79), new(228, 86, 74), new(193, 132, 3), new(0, 132, 188), new(166, 38, 164), new(8, 151, 179) };
    static SKColor[] DiffractogramColorsDark = { new(154, 154, 154), new(152, 195, 121), new(224, 108, 117), new(229, 192, 123), new(97, 175, 240), new(198, 120, 221), new(86, 182, 194) };

    private PlotModel PlotModel { get; set; } = new();
    private Dispatcher UIDispatcher = Dispatcher.UIThread;
    //[Reactive]
    ObservableCollection<string>? ExperimentalsLabel { get; set; }
    //[Reactive]
    int SelectedIndex { get; set; }
    private void ChangeTheme()
    {

        if (Theme)
        {
            PlotModel.TextColor = OxyColors.White;
            PlotModel.PlotAreaBorderColor = OxyColors.White;
            SerieExp = null;
            ProcessPoints();
        }
        else
        {
            PlotModel.TextColor = OxyColors.Black;
            PlotModel.PlotAreaBorderColor = OxyColors.Black;
            SerieExp = null;
            ProcessPoints();
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

    private List<LineSeries>? SerieExp;
    //private LineSeries[] SerieCalc;

    private ObservableCollection<ObservableCollection<DataPoint>>? DataExp;
    OxyColor[] ColorsLight = { OxyColor.FromArgb(255, DiffractogramColorsLigth[0].Red, DiffractogramColorsLigth[0].Green, DiffractogramColorsLigth[0].Blue), OxyColor.FromArgb(255, DiffractogramColorsLigth[2].Red, DiffractogramColorsLigth[2].Green, DiffractogramColorsLigth[2].Blue) };
    OxyColor[] ColorsDark = { OxyColor.FromArgb(255, DiffractogramColorsDark[0].Red, DiffractogramColorsDark[0].Green, DiffractogramColorsDark[0].Blue), OxyColor.FromArgb(255, DiffractogramColorsDark[2].Red, DiffractogramColorsDark[2].Green, DiffractogramColorsDark[2].Blue) };

    private void ProcessPoints()
    {
        if (Data is null || Data.Count == 0)
        {
            ClearDatas();
            return;
        }
        else
        {
            int expIndex = ExperimentalsLabel is null || ExperimentalsLabel.Count == 0 || ExperimentalsLabel.Count == 1 ? 0 : SelectedIndex >= 0 ? SelectedIndex : 0;

            if (SerieExp is null || SerieExp.Count != 1 || DataExp is null || DataExp.Count != 1)
            {
                ClearSeries();
                SerieExp = new()
                {
                    new()
                };
                DataExp = new()
                {
                    new()
                };
                for (int i = 0; i < SerieExp.Count; i++)
                {

                    SerieExp[i] = new()
                    {
                        Color = Theme ? ColorsDark[0] : ColorsLight[0],
                        StrokeThickness = 2,
                    };
                    SerieExp[i].ItemsSource = DataExp[i];
                }
            }

            AddFromArray(SerieExp[0], Data, DataExp[0]);
            //PlotModel.Series.Add(SerieExp[0]);
            if (PlotModel.Series.Count == 0)
                PlotModel.Series.Add(SerieExp[0]);
            else if (PlotModel.Series[0] != SerieExp[0])
                PlotModel.Series[0] = SerieExp[0];

        }

        UpdateChart();
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
                        DataExp[0].Add(new((((double X, double Y))e.NewItems[0]).X, (((double X, double Y))e.NewItems[0]).Y));
                    }
                    else
                    {
                        DataExp[0].Insert(e.NewStartingIndex, new((((double X, double Y))e.NewItems[0]).X, (((double X, double Y))e.NewItems[0]).Y));
                    }
                }
                else
                {

                }
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                DataExp.RemoveAt(e.OldStartingIndex);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                break;

        }
        UpdateChart();

    }
    private void UpdateChart()
    {

        if (PlotModel.DefaultYAxis is not null && (double.IsNaN(PlotModel.DefaultYAxis.FontSize) || PlotModel.DefaultYAxis.FontSize != 0))
            PlotModel.DefaultYAxis.FontSize = 0;
        if (PlotModel.DefaultYAxis is not null && PlotModel.DefaultYAxis.TickStyle != TickStyle.None)
        {
            PlotModel.DefaultYAxis.TickStyle = TickStyle.None;
            PlotModel.DefaultYAxis.MinorTickSize = 0;
            PlotModel.DefaultYAxis.MinorTickSize = 0;
        }
        if (PlotModel.DefaultYAxis is not null && ((Theme && PlotModel.DefaultYAxis.TicklineColor != OxyColors.White) || (!Theme && PlotModel.DefaultYAxis.TicklineColor != OxyColors.Black)))
            PlotModel.DefaultYAxis.TicklineColor = Theme ? OxyColors.White : OxyColors.Black;
        if (PlotModel.DefaultXAxis is not null && ((Theme && PlotModel.DefaultXAxis.TicklineColor != OxyColors.White) || (!Theme && PlotModel.DefaultXAxis.TicklineColor != OxyColors.Black)))
            PlotModel.DefaultXAxis.TicklineColor = Theme ? OxyColors.White : OxyColors.Black;
        UIDispatcher.Post(() =>
        {
            PlotModel.InvalidatePlot(true);
        });
    }
    private static void AddFromArray(XYAxisSeries serie, ObservableCollection<(double X, double Y)> data, ObservableCollection<DataPoint> dataOxy)
    {
        if (data is null)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            if (dataOxy.Count - 1 < i)
                dataOxy.Add(new(data[i].X, data[i].Y));
            else
            {
                dataOxy[i] = new(data[i].X, data[i].Y);
            }
        }
        while (dataOxy.Count > data.Count)
            dataOxy.RemoveAt(dataOxy.Count - 1);

        if (serie.ItemsSource != dataOxy)
            serie.ItemsSource = dataOxy;
    }
    private void ClearDatas()
    {

        if (PlotModel.Series is not null && PlotModel.Series.Count > 0)
        {
            PlotModel.Series.Clear();
        }
    }
    private void ClearSeries()
    {
        for (int i = 0; i < SerieExp?.Count; i++)
        {
            PlotModel.Series.Remove(SerieExp[i]);
        }
    }
}
