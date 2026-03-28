using BaseLibrary;
using LiveChartsCore;
using LiveChartsCore.Measure;
using static GOSAvaloniaControls.GOSChartsBusiness;

namespace GOSAvaloniaControls;

public interface IGOSChartsBusiness
{
    //void SaveImageDiffractogram(double[][]? x, double[][]? y, double[][]? xc, double[][]? yc, double[][]? ba, List<ObservablePoint>[][]? phs, string[]? phsLabel, double[][][]? back, string[]? backLabel, string label, string filePathToSave, LiveChartsBusiness.FormatImage format, int width, int height, double? xmin, double? xmax, double? ymin, double? ymax);
    void SaveToImagePieChart(IEnumerable<ISeries> mainSeries, bool needLigth, string title, string filePathToSave, FormatImage format, LegendPosition legendPosition, int width, int height);
    void SaveToImageCartesianChart(IEnumerable<ISeries> mainSeries, IEnumerable<ISeries>? stackDownSeries, bool needLigth, string title, string xLabel, string yLabel, string filePathToSave, FormatImage format, LegendPosition legendPosition, int width, int height, double? xmin, double? xmax, double? ymin, double? ymax);
    ISeries CopyISerie(ISeries series, bool needLight, double total);
    (string? sharedXfilename, string? otherFilename) SaveToTextCartesianChart(IEnumerable<ISeries> mainSeries, IEnumerable<ISeries>? stackDownSeries, string filePathToSave, string labelX);
    void SaveToTextPieChart(IEnumerable<ISeries> mainSeries, string filePathToSave);
}