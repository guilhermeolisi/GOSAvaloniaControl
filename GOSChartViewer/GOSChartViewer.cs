using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using OxyPlot;
using OxyPlot.Avalonia;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GOSAvaloniaControls;

public partial class GOSChartViewer : TemplatedControl
{
    public static readonly StyledProperty<ObservableCollection<(double X, double Y)>?> DataProperty = AvaloniaProperty.Register<GOSChartViewer, ObservableCollection<(double X, double Y)>?>(nameof(Theme), defaultBindingMode: BindingMode.OneWay);
    public static readonly StyledProperty<bool> ThemeProperty = AvaloniaProperty.Register<GOSChartViewer, bool>(nameof(Theme), true, false, BindingMode.OneWay);

    public ObservableCollection<(double X, double Y)>? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
    /// <summary>
    /// True = Dark; False = Ligth
    /// </summary>
    public bool Theme
    {
        get => GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }
    public GOSChartViewer()
    {

        ThemeProperty.Changed.AddClassHandler<GOSChartViewer>((x, e) => x.ChangeTheme());
        DataProperty.Changed.AddClassHandler<GOSChartViewer>((x, e) => x.ChangeData());
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var _plot = e.NameScope.Find<PlotView>("PART_plotView");
        _plot.Model = PlotModel;
        _plot.Controller = new PlotController();
        _plot.Controller.UnbindAll();
        _plot.Background = new ImmutableSolidColorBrush(Avalonia.Media.Colors.Transparent);
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
    }
    private void ChangeData()
    {
        if (Data is null)
        {
            ClearDatas();
            return;
        }
        SetData(null);
        Data.CollectionChanged += Data_CollectionChanged;
    }
}
