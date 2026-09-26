using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using FluentAssertions;
using GOSAvaloniaControls;

namespace GOSCustomControl.Tests;

/// <summary>
/// O quadrado e medido pelo layout, com as duas dimensoes mandando. O defeito que isto fixa e o do handler antigo do
/// Results3DImageView, que so re-quadrava quando a ALTURA mudava.
/// <para>Mede e arranja o decorador direto, pelo protocolo de layout, sem janela: a aplicacao de teste nao tem tema, e
/// sem tema a <c>Window</c> nao tem template para hospedar o conteudo.</para>
/// </summary>
public class SquareDecoratorTests
{
    private static void Layout(Layoutable control, double width, double height)
    {
        control.Measure(new Size(width, height));
        control.Arrange(new Rect(0, 0, width, height));
    }

    [AvaloniaFact]
    public void Medir_ComLarguraMaiorQueAltura_OLadoEAAltura()
    {
        var child = new Border();
        var decorator = new SquareDecorator { Child = child };

        Layout(decorator, 1000, 600);

        decorator.DesiredSize.Should().Be(new Size(600, 600));
        child.Bounds.Size.Should().Be(new Size(600, 600));
        child.Bounds.X.Should().Be(200, "a sobra de largura fica dos dois lados");
    }

    [AvaloniaFact]
    public void Medir_ComAlturaMaiorQueLargura_OLadoEALargura()
    {
        var child = new Border();
        var decorator = new SquareDecorator { Child = child };

        Layout(decorator, 600, 1000);

        child.Bounds.Size.Should().Be(new Size(600, 600));
        child.Bounds.Y.Should().Be(200, "a sobra de altura fica em cima e embaixo");
    }

    [AvaloniaFact]
    public void MudarSoALargura_ReMede()
    {
        var child = new Border();
        var decorator = new SquareDecorator { Child = child };
        Layout(decorator, 1000, 600);

        Layout(decorator, 500, 600);

        decorator.DesiredSize.Should().Be(new Size(500, 500));
        child.Bounds.Size.Should().Be(new Size(500, 500), "a largura tambem manda no lado");
    }

    [AvaloniaFact]
    public void ComPadding_OQuadradoFicaDentroDoPadding()
    {
        var child = new Border();
        var decorator = new SquareDecorator { Child = child, Padding = new Thickness(10) };

        Layout(decorator, 1000, 620);

        decorator.DesiredSize.Should().Be(new Size(620, 620));
        child.Bounds.Should().Be(new Rect(200, 10, 600, 600));
    }
}
