using FluentAssertions;
using GOSAvaloniaControls;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;

namespace GOSChartViewerModel.Tests;

public sealed class GOSChartsBusinessTests
{
    // O titulo do print do achado 140 (E089): a descricao da imagem 3D de RMSD do modelo de Wilkens, com o L e o plano.
    private const string LongTitle =
        "Dislocation. Root mean square displacement <dL_hkl^2(L)>^1/2 = L <eps_hkl^2(L)>^1/2 by direction, in A. "
        + "Wilkens model, rho 1E-05 1/A2, Re 50 A, b 3.864 A. Fourier length L (A) = 100. "
        + "Projection (positive) on lattice plane (1 0 0)";

    private const int ExportWidth = 1600;

    /// <summary>Mede o titulo como o LiveCharts o desenha, com os mesmos texto, tamanho, margem e largura maxima.</summary>
    private static LvcSize Measure(string text, float maxWidth)
    {
        var geometry = new LabelGeometry
        {
            Text = text,
            TextSize = GOSChartsBusiness.ExportTitleTextSize,
            Padding = new Padding(GOSChartsBusiness.ExportTitlePadding),
            MaxWidth = maxWidth,
            Paint = new SolidColorPaint(0xff303030),
        };
        return geometry.Measure();
    }

    [Fact]
    public void CreateExportTitle_ShouldWrapInsideTheImage_WhenTheTitleIsLongerThanTheExportWidth()
    {
        // Arrange: sem largura maxima, o titulo do print passa dos 1600 px da imagem exportada (e o que o cortava)
        LvcSize singleLine = Measure(LongTitle, float.MaxValue);
        singleLine.Width.Should().BeGreaterThan(ExportWidth, "o titulo do print nao cabe numa linha so de 1600 px");

        // Act
        var title = GOSChartsBusiness.CreateExportTitle(LongTitle, ExportWidth);
        LvcSize wrapped = Measure(title.Text, (float)title.MaxWidth);

        // Assert
        title.Text.Should().Be(LongTitle, "o export leva o titulo inteiro");
        wrapped.Width.Should().BeLessThanOrEqualTo(ExportWidth, "o titulo quebra a linha dentro da largura da imagem");
        wrapped.Height.Should().BeGreaterThan(singleLine.Height * 1.5f, "o titulo longo ocupa mais de uma linha");
    }

    [Fact]
    public void CreateExportTitle_ShouldKeepOneLine_WhenTheTitleFits()
    {
        var title = GOSChartsBusiness.CreateExportTitle("Size", ExportWidth);

        LvcSize size = Measure(title.Text, (float)title.MaxWidth);

        size.Height.Should().BeApproximately(Measure("Size", float.MaxValue).Height, 0.5f, "titulo curto nao ganha linha");
    }
}
