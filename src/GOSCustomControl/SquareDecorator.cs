using Avalonia;
using Avalonia.Controls;

namespace GOSAvaloniaControls;

/// <summary>
/// Da ao filho um quadrado cujo lado e o MENOR entre a largura e a altura disponiveis, centralizado na area recebida.
/// <para>Layout puro, sem evento e sem dependencia do conteudo: as duas dimensoes mandam na medida, entao mudar so a
/// largura re-mede. Substitui o padrao antigo de forcar <c>Width = altura</c> num handler de <c>SizeChanged</c>, em que
/// so a altura mandava e a largura que sobrava virava vao morto.</para>
/// <para>O decorador estica na celula que o contem e centraliza o quadrado: a sobra fica dos dois lados, e quem quiser
/// usa-la pode por uma coluna <c>Auto</c> ao lado.</para>
/// </summary>
public class SquareDecorator : Decorator
{
    protected override Size MeasureOverride(Size availableSize)
    {
        Size inner = availableSize.Deflate(Padding);
        double side = Side(inner);
        if (double.IsInfinity(side))
        {
            // Sem limite nas duas direcoes (dentro de um ScrollViewer, por exemplo): o filho diz o tamanho dele.
            if (Child is null)
                return new Size(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            Child.Measure(inner);
            side = Math.Max(Child.DesiredSize.Width, Child.DesiredSize.Height);
        }
        else
        {
            Child?.Measure(new Size(side, side));
        }
        return new Size(side, side).Inflate(Padding);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Size inner = finalSize.Deflate(Padding);
        double side = Math.Min(inner.Width, inner.Height);
        Child?.Arrange(new Rect(Padding.Left + (inner.Width - side) / 2, Padding.Top + (inner.Height - side) / 2, side, side));
        return finalSize;
    }

    /// <summary>O menor lado finito; infinito so quando as duas dimensoes sao infinitas.</summary>
    private static double Side(Size inner)
    {
        if (double.IsInfinity(inner.Width))
            return inner.Height;
        if (double.IsInfinity(inner.Height))
            return inner.Width;
        return Math.Min(inner.Width, inner.Height);
    }
}
