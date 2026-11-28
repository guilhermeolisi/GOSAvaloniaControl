using BaseLibrary;
using FluentAssertions;
using GOSAvaloniaControls;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace GOSChartViewerModel.Tests;

/// <summary>E118, P3: os graficos viram PNG em memoria, sem arquivo, no tamanho pedido.</summary>
public sealed class GOSChartsBusinessInMemoryRenderTests
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private readonly GOSChartsBusiness business = new(new ColorServices(), new FileServices());

    private static ISeries[] Lines() =>
    [
        new LineSeries<ObservablePoint> { Name = "Observed", Values = [new(10, 5), new(20, 40), new(30, 12)], GeometrySize = 0 },
        new LineSeries<ObservablePoint> { Name = "Calculated", Values = [new(10, 6), new(20, 38), new(30, 11)], GeometrySize = 0 },
    ];

    private static ISeries[] Slices() =>
    [
        new PieSeries<double> { Name = "Quartz", Values = [70] },
        new PieSeries<double> { Name = "Calcite", Values = [30] },
    ];

    /// <summary>Quantos pixels do PNG nao sao o fundo branco: imagem vazia da zero.</summary>
    private static int InkedPixels(SKBitmap bitmap)
    {
        int count = 0;
        for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
                if (bitmap.GetPixel(x, y) != SKColors.White)
                    count++;
        return count;
    }

    [Theory]
    [InlineData(1200, 700, 1)]
    [InlineData(2600, 1500, 2.5)]
    public void RenderCartesianChartPng_ShouldReturnPngOfTheRequestedSize(int width, int height, double textScale)
    {
        // Act
        byte[] png = business.RenderCartesianChartPng(Lines(), "Fit", "2θ (°)", "Intensity", LegendPosition.Bottom, width, height,
            null, null, null, null, textScale);

        // Assert
        png.Take(PngSignature.Length).Should().Equal(PngSignature);
        using SKBitmap bitmap = SKBitmap.Decode(png);
        bitmap.Width.Should().Be(width);
        bitmap.Height.Should().Be(height);
        InkedPixels(bitmap).Should().BeGreaterThan(width, "as curvas, os eixos e o titulo estao desenhados");
    }

    [Theory]
    [InlineData(900, 900, 1)]
    [InlineData(1800, 1600, 2)]
    public void RenderPieChartPng_ShouldReturnPngOfTheRequestedSize(int width, int height, double textScale)
    {
        // Act
        byte[] png = business.RenderPieChartPng(Slices(), "Weight fractions", LegendPosition.Right, width, height, textScale);

        // Assert
        png.Take(PngSignature.Length).Should().Equal(PngSignature);
        using SKBitmap bitmap = SKBitmap.Decode(png);
        bitmap.Width.Should().Be(width);
        bitmap.Height.Should().Be(height);
        InkedPixels(bitmap).Should().BeGreaterThan(width * height / 10, "as fatias ocupam boa parte da imagem");
    }
}
