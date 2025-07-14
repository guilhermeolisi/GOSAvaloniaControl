using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Avalonia;
using System.Collections;
using System.Collections.Specialized;

namespace GOSAvaloniaControls;

public partial class GOSCartesian : TemplatedControl
{
    //public static readonly StyledProperty<ObservableCollection<(double X, double Y)>?> DataProperty = AvaloniaProperty.Register<GOSCartesian, ObservableCollection<(double X, double Y)>?>(nameof(Data), defaultBindingMode: BindingMode.OneWay);
    public static readonly StyledProperty<IEnumerable?> DataProperty = AvaloniaProperty.Register<GOSCartesian, IEnumerable?>(nameof(Data), defaultBindingMode: BindingMode.OneWay);
    public static readonly StyledProperty<bool> IsDarkThemeProperty = AvaloniaProperty.Register<GOSCartesian, bool>(nameof(IsDarkTheme), false, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> IsVerticalLineProperty = AvaloniaProperty.Register<GOSCartesian, bool>(nameof(IsVerticalLine), false, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> IsZoomingProperty = AvaloniaProperty.Register<GOSCartesian, bool>(nameof(IsZooming), false, false, BindingMode.OneWay);
    public static readonly StyledProperty<string> XlabelProperty = AvaloniaProperty.Register<GOSCartesian, string>(nameof(XLabel), "X", false, BindingMode.OneWay);
    public static readonly StyledProperty<string> YlabelProperty = AvaloniaProperty.Register<GOSCartesian, string>(nameof(YLabel), "Y", false, BindingMode.OneWay);


    //public ObservableCollection<(double X, double Y)>? Data
    //{
    //    get => GetValue(DataProperty);
    //    set => SetValue(DataProperty, value);
    //}
    public IEnumerable? Data
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
    public bool IsVerticalLine
    {
        get => GetValue(IsVerticalLineProperty);
        set => SetValue(IsVerticalLineProperty, value);
    }
    public bool IsZooming
    {
        get => GetValue(IsZoomingProperty);
        set => SetValue(IsZoomingProperty, value);
    }
    public string XLabel
    {
        get => GetValue(XlabelProperty);
        set => SetValue(XlabelProperty, value);
    }
    public string YLabel
    {
        get => GetValue(YlabelProperty);
        set => SetValue(YlabelProperty, value);
    }

    public GOSCartesian()
    {

        IsDarkThemeProperty.Changed.AddClassHandler<GOSCartesian>((x, e) => x.ChangeTheme());
        DataProperty.Changed.AddClassHandler<GOSCartesian>((x, e) => x.ChangeData(e));
        IsZoomingProperty.Changed.AddClassHandler<GOSCartesian>((x, e) => x.ChangeZoom());
        XlabelProperty.Changed.AddClassHandler<GOSCartesian>((x, e) => x.ChangeXLabel());
        YlabelProperty.Changed.AddClassHandler<GOSCartesian>((x, e) => x.ChangeYLabel());

        Series[0].Values = DataPoints;

    }
    CartesianChart _chart;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _chart = e.NameScope.Find<CartesianChart>("PART_Chart");
        _chart.Series = Series;
        _chart.XAxes = Axes[0];
        _chart.YAxes = Axes[1];
        _chart.DrawMarginFrame = DrawMarginFrame;

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
        ChangeXLabel();
        ChangeYLabel();
        ChangeData(null);
        ChangeZoom();

    }
    private void ChangeData(AvaloniaPropertyChangedEventArgs? e)
    {
        if (Data is null)
        {
            ClearDatas();
            return;
        }
        //Data.CollectionChanged -= Data_CollectionChanged;
        //SetData(null);
        //Data.CollectionChanged += Data_CollectionChanged;
        if (e is not null && e.OldValue is INotifyCollectionChanged old)
        {
            old.CollectionChanged -= Data_CollectionChanged;
        }
        INotifyCollectionChanged? data = e is null || e.NewValue is null ? Data as INotifyCollectionChanged : e.NewValue as INotifyCollectionChanged;
        if (data is not null)
        {
            data.CollectionChanged -= Data_CollectionChanged;
            SetData(null);
            data.CollectionChanged += Data_CollectionChanged;
        }
    }
    private void ChangeZoom()
    {
        if (_chart is null)
            return;
        _chart.ZoomMode = IsZooming ? ZoomAndPanMode.X : ZoomAndPanMode.None;
        ResetZoom();
    }
    private void ChangeXLabel()
    {
        if (_chart is null)
            return;
        _chart.XAxes.First().Name = XLabel;
    }
    private void ChangeYLabel()
    {
        if (_chart is null)
            return;
        _chart.YAxes.First().Name = YLabel;
    }
}
