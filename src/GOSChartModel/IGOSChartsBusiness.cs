using LiveChartsCore;
using LiveChartsCore.Measure;
using static GOSAvaloniaControls.GOSChartsBusiness;

namespace GOSAvaloniaControls;

public interface IGOSChartsBusiness
{
    //void SaveImageDiffractogram(double[][]? x, double[][]? y, double[][]? xc, double[][]? yc, double[][]? ba, List<ObservablePoint>[][]? phs, string[]? phsLabel, double[][][]? back, string[]? backLabel, string label, string filePathToSave, LiveChartsBusiness.FormatImage format, int width, int height, double? xmin, double? xmax, double? ymin, double? ymax);
    void SaveImagePieChart(IEnumerable<ISeries> mainSeries, string title, string filePathToSave, FormatImage format, int width, int height, bool needLigth, LegendPosition legendPosition);
}