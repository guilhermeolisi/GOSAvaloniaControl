using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Extensions;
using System.Collections.ObjectModel;

namespace GOSAvaloniaControls;

public partial class GOSPieChart : TemplatedControl
{
    public static readonly StyledProperty<ObservableCollection<double>?> DataProperty = AvaloniaProperty.Register<GOSPieChart, ObservableCollection<double>?>(nameof(Data), null, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> IsDarkThemeProperty = AvaloniaProperty.Register<GOSPieChart, bool>(nameof(IsDarkTheme), true, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> ShowPercentToolTipProperty = AvaloniaProperty.Register<GOSPieChart, bool>(nameof(ShowPercentToolTip), true, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> ShowValueToolTipProperty = AvaloniaProperty.Register<GOSPieChart, bool>(nameof(ShowValueToolTip), true, false, BindingMode.OneWay);
    public static readonly StyledProperty<ObservableCollection<string>?> LabelsProperty = AvaloniaProperty.Register<GOSPieChart, ObservableCollection<string>?>(nameof(Labels), null, false, BindingMode.OneWay);
    public static readonly StyledProperty<byte> ShowLegendProperty = AvaloniaProperty.Register<GOSPieChart, byte>(nameof(ShowLegend), 0, false, BindingMode.OneWay);
    public static readonly StyledProperty<string?> StringFormatValueProperty = AvaloniaProperty.Register<GOSPieChart, string?>(nameof(StringFormatValue), null, false, BindingMode.OneWay);



    public ObservableCollection<double>? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
    /// <summary>
    /// True = Dark; False = Ligth
    /// </summary>
    public bool IsDarkTheme
    {
        get => GetValue(IsDarkThemeProperty);
        set => SetValue(IsDarkThemeProperty, value);
    }
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
    public ObservableCollection<string>? Labels
    {
        get => GetValue(LabelsProperty);
        set => SetValue(LabelsProperty, value);
    }
    /// <summary>
    /// 0 = Hidden, 1 = Left, 2 = Top, 3 = Right, 4 = Bottom
    /// </summary>
    public byte ShowLegend
    {
        get => GetValue(ShowLegendProperty);
        set => SetValue(ShowLegendProperty, value);
    }
    public string? StringFormatValue
    {
        get => GetValue(StringFormatValueProperty);
        set => SetValue(StringFormatValueProperty, value);
    }


    public GOSPieChart()
    {

        IsDarkThemeProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.ChangeTheme());
        ShowValueToolTipProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.SetData());
        ShowPercentToolTipProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.SetData());
        DataProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.ChangeData());
        LabelsProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.ChangeLabels());
        ShowLegendProperty.Changed.AddClassHandler<GOSPieChart>((x, e) => x.ChangeShowLegend());
    }
    PieChart _chart;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _chart = e.NameScope.Find<PieChart>("PART_Chart");
        _chart.Series = DataToShow;

        //ChangeData();
        //var wrap = e.NameScope.Find<ToggleButton>("PART_WrapCheck");
        //var edit = e.NameScope.Find<ToggleButton>("PART_EditCheck");

        //https://github.com/AvaloniaUI/Avalonia/issues/4616
        //http://reference.avaloniaui.net/api/Avalonia.Controls/ResourceDictionary/50FEA02D
        //if (Application.Current.TryFindResource(""))
        //{
        //    wrap[ForegroundProperty] = new DynamicResourceExtension("MyResource");
        //}

        ChangeTheme();
        ChangeLabels();
        ChangeData();
        ChangeShowLegend();
    }
    private void ChangeData()
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
    private void ChangeLabels()
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
    private void ChangeShowLegend()
    {
        if (_chart is null)
            return;
        if (Labels is null || Data is null || Labels.Count != Data.Count)
        {
            _chart.LegendPosition = LegendPosition.Hidden;
            return;
        }

        _chart.Legend = IsDarkTheme ? new LiveLegendDark(true) : new LiveLegendLigth(true);

        _chart.LegendPosition = ShowLegend switch
        {
            0 => LegendPosition.Hidden,
            1 => LegendPosition.Left,
            2 => LegendPosition.Top,
            3 => LegendPosition.Right,
            4 => LegendPosition.Bottom,
            _ => LegendPosition.Hidden
        };

        _chart.CoreChart.Update(new LiveChartsCore.Kernel.ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
        //&& Labels is not null && Labels.Count == Data.Count ? LegendPosition.Right : LegendPosition.Hidden;
    }
}
