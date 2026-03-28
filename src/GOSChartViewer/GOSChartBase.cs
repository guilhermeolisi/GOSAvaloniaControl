using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Metadata;
using BaseLibrary.Collections;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Avalonia;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using static GOSAvaloniaControls.GOSChartsBusiness;

namespace GOSAvaloniaControls;

public class GOSChartBase : TemplatedControl
{
    public static readonly StyledProperty<string?> TitleProperty = AvaloniaProperty.Register<GOSChartBase, string?>(nameof(Title), null, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> IsDarkThemeProperty = AvaloniaProperty.Register<GOSChartBase, bool>(nameof(IsDarkTheme), true, false, BindingMode.OneWay);
    public static readonly StyledProperty<byte> ShowLegendProperty = AvaloniaProperty.Register<GOSChartBase, byte>(nameof(ShowLegend), 0, false, BindingMode.OneWay);
    public static readonly StyledProperty<string?> StringFormatValueProperty = AvaloniaProperty.Register<GOSChartBase, string?>(nameof(StringFormatValue), null, false, BindingMode.OneWay);
    public static readonly StyledProperty<string?> FilePathToSaveProperty = AvaloniaProperty.Register<GOSChartBase, string?>(nameof(FilePathToSave), null, false, BindingMode.OneWay);
    public static readonly StyledProperty<ObservableCollection<string>?> LabelsProperty = AvaloniaProperty.Register<GOSChartBase, ObservableCollection<string>?>(nameof(Labels), null, false, BindingMode.OneWay);
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    /// <summary>
    /// True = Dark; False = Ligth
    /// </summary>
    public bool IsDarkTheme
    {
        get => GetValue(IsDarkThemeProperty);
        set => SetValue(IsDarkThemeProperty, value);
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
    public string? FilePathToSave
    {
        get => GetValue(FilePathToSaveProperty);
        set => SetValue(FilePathToSaveProperty, value);
    }
    public ObservableCollection<string>? Labels
    {
        get => GetValue(LabelsProperty);
        set => SetValue(LabelsProperty, value);
    }

    static GOSChartBase()
    {
        IsDarkThemeProperty.Changed.AddClassHandler<GOSChartBase>((x, e) => x.ChangeTheme());
        ShowLegendProperty.Changed.AddClassHandler<GOSChartBase>((x, e) => x.ChangeShowLegend());
        LabelsProperty.Changed.AddClassHandler<GOSChartBase>((x, e) => x.ChangeLabels());
        // Agora usa x. corretamente — sem capturar 'this' do construtor
        FilePathToSaveProperty.Changed.AddClassHandler<GOSChartBase>((x, e) => x.ChangeFilePathToSave());
    }

    public GOSChartBase()
    {
        _saveToFileRelayCommand = new GOSRelayCommand(
        param => ExecuteSaveToFile((string?)param),
        param => /*(string?)param != "txt" && */!string.IsNullOrWhiteSpace(FilePathToSave));


        // Assina mudanças da StyledProperty e força RaiseCanExecuteChanged quando mudar
        //this.GetObservable(MessageProperty).Subscribe(_ =>
        //{
        //    (_performCommand as RelayCommand)?.RaiseCanExecuteChanged();
        //});
#if DEBUG
        _ = FilePathToSave;
        //FilePathToSave = string.Empty;
#endif
    }
    protected virtual IChartView _chartBase { get; }
    protected virtual IEnumerable<ISeries>? _series { get; }
    protected virtual IEnumerable? _data { get; }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        // Wire commands diretamente — RelativeSource não funciona dentro de ContextFlyout/Popup
        
        if ((_chartBase as UserControl)!.ContextFlyout is MenuFlyout menuFlyout)
        {
            foreach (var item in menuFlyout.Items.OfType<MenuItem>())
                item.Command = SaveToFileCommand;
        }

        //ChangeTheme();
        //ChangeData();
        //ChangeShowLegend();

        _saveToFileRelayCommand?.RaiseCanExecuteChanged();

#if DEBUG
        _ = FilePathToSave;
        //FilePathToSave = string.Empty;
#endif
    }
    protected virtual void ChangeData(AvaloniaPropertyChangedEventArgs? e)
    {
        throw new NotImplementedException();
    }
    protected virtual void ChangeShowLegend()
    {
        if (_chartBase is null)
            return;
        if (Labels is null || _data is null || Labels.Count != _data.Count())
        {
            _chartBase.LegendPosition = LegendPosition.Hidden;
            return;
        }

        _chartBase.Legend = IsDarkTheme ? new LiveLegendDark(true) : new LiveLegendLigth(true);

        _chartBase.LegendPosition = ShowLegend switch
        {
            0 => LegendPosition.Hidden,
            1 => LegendPosition.Left,
            2 => LegendPosition.Top,
            3 => LegendPosition.Right,
            4 => LegendPosition.Bottom,
            _ => LegendPosition.Hidden
        };

        _chartBase.CoreChart.Update(new LiveChartsCore.Kernel.ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
        //&& Labels is not null && Labels.Count == Data.Count ? LegendPosition.Right : LegendPosition.Hidden;
    }

    private IGOSChartsBusiness _chartBusiness = new GOSChartsBusiness();

    // Substitui o método público e o [DependsOn] pela propriedade ICommand
    private GOSRelayCommand _saveToFileRelayCommand;
    public ICommand SaveToFileCommand => _saveToFileRelayCommand; 

    private void ExecuteSaveToFile(string? param)
    {
        if (param is null) return;
        string? pathToSave = FilePathToSave;
        if (string.IsNullOrWhiteSpace(pathToSave))
            return;
        if (!string.IsNullOrWhiteSpace(Title))
        {
            pathToSave = Path.Combine(Path.GetDirectoryName(pathToSave)!, Path.GetFileNameWithoutExtension(pathToSave) + " - " + Title + "." + param);
        }
        if (this is GOSPieChart)
        {
            if (param == "txt")
            {
                _chartBusiness.SaveToTextPieChart(_series, pathToSave);
            }
            else
            {
                _chartBusiness.SaveToImagePieChart(_series, IsDarkTheme, Title, pathToSave, param == "png" ? FormatImage.PNG : FormatImage.SVG, _chartBase.LegendPosition, 1600, 900);
            }
        }
        else if (this is GOSCartesian)
        {
            CartesianChart cart = _chartBase as CartesianChart;
            if (param == "txt") 
            {
                _chartBusiness.SaveToTextCartesianChart(_series, null, pathToSave, cart.XAxes.FirstOrDefault()?.Name);
            }
            else
            {
                _chartBusiness.SaveToImageCartesianChart(_series, null, IsDarkTheme, Title, cart.XAxes.FirstOrDefault()?.Name, cart.YAxes.FirstOrDefault()?.Name, pathToSave, param == "png" ? FormatImage.PNG : FormatImage.SVG, _chartBase.LegendPosition, 1600, 900, null, null, null, null);
            }
        }
    }

    private void ChangeFilePathToSave()
    {
        // Notifica o ICommand que o CanExecute deve ser reavaliado
        _saveToFileRelayCommand?.RaiseCanExecuteChanged();
#if DEBUG
        _ = FilePathToSave;
        //FilePathToSave = string.Empty;
#endif
    }
    //// Fallback explícito: ICommand wrapper que usa os mesmos métodos
    //private RelayCommand? _performCommand;
    //public ICommand PerformCommand => _performCommand ??= new RelayCommand(
    //    _ => PerformAction(_),
    //    _ => CanPerformAction(_)
    //);
    protected virtual void ChangeTheme()
    {
        throw new NotImplementedException();
    }
    protected virtual void ChangeLabels()
    {
        throw new NotImplementedException();
    }
}

