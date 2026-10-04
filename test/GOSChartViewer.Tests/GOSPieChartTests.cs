using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using FluentAssertions;
using GOSAvaloniaControls;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace GOSChartViewer.Tests;

public sealed class GOSPieChartTests
{
    [AvaloniaFact]
    public void ChangeData_ShouldLabelEachSliceByPosition_WhenTwoSlicesHaveTheSameValue()
    {
        // E089 (A022): o rotulo da fatia vinha de Data.IndexOf(valor), e duas fases 50/50 saiam as duas com o nome da
        // primeira. O nome tem de vir da posicao da fatia.
        var pie = new GOSPieChart
        {
            Labels = new ObservableCollection<string>(["A", "B"]),
            Data = new ObservableCollection<double>([50, 50]),
        };
        var window = new Window { Content = pie, Width = 400, Height = 400 };
        window.Show();

        PieChart chart = pie.GetVisualDescendants().OfType<PieChart>().Single();

        chart.Series.Should().NotBeNull();
        chart.Series!.Select(s => s.Name).Should().Equal("A", "B");
    }
}
