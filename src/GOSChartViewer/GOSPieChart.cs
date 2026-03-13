using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Extensions;
using System.Collections;
using System.Collections.ObjectModel;
using static GOSAvaloniaControls.GOSChartsBusiness;

namespace GOSAvaloniaControls;

public partial class GOSPieChart : GOSChartBase
{
    public static readonly StyledProperty<ObservableCollection<double>?> DataProperty = AvaloniaProperty.Register<GOSPieChart, ObservableCollection<double>?>(nameof(Data), null, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> ShowPercentToolTipProperty = AvaloniaProperty.Register<GOSPieChart, bool>(nameof(ShowPercentToolTip), true, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> ShowValueToolTipProperty = AvaloniaProperty.Register<GOSPieChart, bool>(nameof(ShowValueToolTip), true, false, BindingMode.OneWay);
    public ObservableCollection<double>? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
    protected override IEnumerable? _data => Data;
    protected override IEnumerable<ISeries>? _series => _chart?.Series;
    protected override IChartView _chartBase => _chart;
    public bool ShowPercentToolTip
    {
        get => GetValue(ShowPercentToolTipProperty);
        set => SetValue(ShowPercentToolTipProperty, value);
    }
    public bool ShowValueToolTip
    {
        get => GetValue(ShowValueToolTipProperty);
        set => SetValue(ShowValueToolTipProperty, value);
    }

    public GOSPieChart()
    {

        ShowValueToolTipProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.SetData());
        ShowPercentToolTipProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.SetData());
        DataProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.ChangeData(e));
        ShowLegendProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.ChangeShowLegend());
        //FilePathToSaveProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.ChangeFilePathToSave());
    }
    PieChart _chart;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _chart = e.NameScope.Find<PieChart>("PART_Chart");
        _chart.Series = DataToShow;
        
        
        base.OnApplyTemplate(e);

        
        
        ChangeTheme();
        ChangeLabels();
        ChangeData(null);
        ChangeShowLegend();
    }
    protected override void ChangeData(AvaloniaPropertyChangedEventArgs? e)
    {
        if (_chart is null)
            return;

        if (Data is null)
        {
            if (_chart.Series is not null && _chart.Series.Count() > 0)
                _chart.Series = [];
            return;
        }

        _chart.Series = Data.AsPieSeries((value, series) =>
            {

                //https://livecharts.dev/docs/Avalonia/2.0.0-rc2/samples.pies.outlabels

                int index = Data.IndexOf(value);

                series.Name = Labels is null || index >= Labels.Count ? string.Empty : Labels[index];

                //https://livecharts.dev/docs/Avalonia/2.0.0-rc2/samples.pies.custom
                // this method is called once per element in the array, so:

                // for the series with the value 6, we set the OuterRadiusOffset to 0
                // for the series with the value 5, the OuterRadiusOffset is 50
                // for the series with the value 4, the OuterRadiusOffset is 100
                // for the series with the value 3, the OuterRadiusOffset is 150

                //series.Name = value == CrystallineArea.Value ? "Cryst" ;

                //series.OuterRadiusOffset = outer;
                //double changeOuter = 200 / Results.Phases.Count;
                //if (changeOuter > 40)
                //    changeOuter = 40;
                //outer += changeOuter;

                //series.DataLabelsPaint = new SolidColorPaint(SKColors.White)
                //{
                //    //SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                //};


                series.ToolTipLabelFormatter =
                    point =>
                    {
                        var pv = point.Coordinate.PrimaryValue;
                        var sv = point.StackedValue!;

                        var a = (ShowValueToolTip ? pv.ToString(string.IsNullOrEmpty(StringFormatValue) ? string.Empty : StringFormatValue) : string.Empty)
                            + (ShowValueToolTip && ShowPercentToolTip ? "(" : string.Empty)
                            + (ShowPercentToolTip ? sv.Share.ToString("P2") : string.Empty)
                            + (ShowValueToolTip && ShowPercentToolTip ? ")" : string.Empty);
                        return a;
                    };
            });
        //if (Data is null)
        //{
        //    ClearDatas();
        //    return;
        //}
        //SetData();
        //Data.CollectionChanged += Data_CollectionChanged;
    }
    protected override void ChangeLabels()
    {
        if (_chart is null)
            return;

        if (Labels is null || Data is null || Labels.Count != Data.Count)
        {
            return;
        }

        for (int i = 0; i < DataToShow.Count; i++)
        {
            DataToShow[i].Name = Labels[i];
        }

        //Forçar o Update
        _chart.CoreChart.Update(new LiveChartsCore.Kernel.ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
        //ChangeDataToObservableCollection(Data, DataToShow);
        ChangeShowLegend();
    }
    //private void ChangeShowLegend()
    //{
    //    if (_chart is null)
    //        return;
    //    if (Labels is null || Data is null || Labels.Count != Data.Count)
    //    {
    //        _chart.LegendPosition = LegendPosition.Hidden;
    //        return;
    //    }

    //    _chart.Legend = IsDarkTheme ? new LiveLegendDark(true) : new LiveLegendLigth(true);

    //    _chart.LegendPosition = ShowLegend switch
    //    {
    //        0 => LegendPosition.Hidden,
    //        1 => LegendPosition.Left,
    //        2 => LegendPosition.Top,
    //        3 => LegendPosition.Right,
    //        4 => LegendPosition.Bottom,
    //        _ => LegendPosition.Hidden
    //    };

    //    _chart.CoreChart.Update(new LiveChartsCore.Kernel.ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
    //    //&& Labels is not null && Labels.Count == Data.Count ? LegendPosition.Right : LegendPosition.Hidden;
    //}

    //private IGOSChartsBusiness _chartBusiness = new GOSChartsBusiness();

    //public void SaveToFileCommand(object parameter)
    //{
    //    string param = (string)parameter;
    //    if (param == "txt")
    //    {

    //    }
    //    else
    //    {
    //        _chartBusiness.SaveImagePieChart(_chart.Series, null, FilePathToSave, parameter == "png" ? FormatImage.PNG : FormatImage.SVG, (int)Width, (int)Height, IsDarkTheme, _chart.LegendPosition);
    //    }
    //}
    //public bool CanSaveToFileCommand(object msg)
    //{

    //    return msg as string == "txt" ? false : !string.IsNullOrWhiteSpace(FilePathToSave);
    //}
}
