using FluentAssertions;

using GOSAvaloniaControls;

namespace GOSCustomControl.Tests;

/// <summary>
/// A notificacao com duas acoes (E183, lei 6.7 do NIMLOTH-UX.md): o balao mostra a primeira, a lista do sino as duas.
/// </summary>
public class NotificationItemTests
{
    [Fact]
    public void Constructor_ShouldKeepBothActions_WhenTwoAreGiven()
    {
        int copied = 0, opened = 0;

        var item = new NotificationItem(3, "The walk stopped unexpectedly.", true, "Copy details", () => copied++,
            "Open log folder", () => opened++);

        item.HasAction.Should().BeTrue();
        item.ActionText.Should().Be("Copy details");
        item.HasSecondAction.Should().BeTrue();
        item.SecondActionText.Should().Be("Open log folder");

        item.ActionCommand!.Execute(null);
        item.SecondActionCommand!.Execute(null);
        copied.Should().Be(1);
        opened.Should().Be(1);
    }

    [Fact]
    public void Constructor_ShouldHaveNoSecondAction_WhenOnlyOneIsGiven()
    {
        var item = new NotificationItem(1, "Walk finished.", true, "Open results", () => { });

        item.HasAction.Should().BeTrue();
        item.HasSecondAction.Should().BeFalse();
        item.SecondActionText.Should().BeNull();
        item.SecondActionCommand.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldRefuseAnEmptySecondAction()
    {
        var act = () => new NotificationItem(3, "m", true, "Copy details", () => { }, " ", () => { });

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DefaultInterfaceMember_ShouldFallBackToTheOneActionOverload_WhenTheImplementationIgnoresTheSecond()
    {
        // Implementacao antiga (sem a sobrecarga nova) continua recebendo a notificacao, com a primeira acao.
        var bell = new OneActionBell();
        IGOSNotification notification = bell;
        Action copy = () => { };

        notification.AddNotification(3, "The walk stopped unexpectedly.", true, "Copy details", copy, "Open log folder", () => { });

        bell.Received.Should().Equal(("The walk stopped unexpectedly.", "Copy details"));
    }

    private sealed class OneActionBell : IGOSNotification
    {
        public List<(string Message, string? Action)> Received { get; } = [];
        public bool UIContextIsNull => true;
        public void AddNotification(byte severity, string message, bool showBallon) => Received.Add((message, null));
        public void AddNotification(byte severity, string message, bool showBallon, string actionText, Action action)
            => Received.Add((message, actionText));
        public void SetUIContext(SynchronizationContext? uiContext) { }
    }
}
