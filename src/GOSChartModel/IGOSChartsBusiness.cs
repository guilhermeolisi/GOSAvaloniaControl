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

    /// <summary>
    /// O grafico cartesiano como PNG em memoria, no tema claro e sem arquivo (E118: as figuras do laudo).
    /// <paramref name="textScale"/> multiplica o texto do titulo, dos eixos e da legenda: a imagem larga impressa em
    /// poucos centimetros pede letra maior que a da tela.
    /// </summary>
    byte[] RenderCartesianChartPng(IEnumerable<ISeries> series, string title, string xLabel, string yLabel, LegendPosition legend, int width, int height, double? xmin, double? xmax, double? ymin, double? ymax, double textScale = 1);

    /// <summary>A pizza como PNG em memoria, no tema claro e sem arquivo (E118). <paramref name="textScale"/> como no cartesiano.</summary>
    byte[] RenderPieChartPng(IEnumerable<ISeries> series, string title, LegendPosition legend, int width, int height, double textScale = 1);
}