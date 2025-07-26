using System.Collections;

namespace GOSAvaloniaControls.NavigationBar.Model;

public class GOSNavigationBarTree
{
    public string? Caption { get; set; }
    public string? CaptionChildren { get; set; }
    public List<GOSNavigationBarTree> Children { get; private set; } = new();
    public Action? Notifier { get; private set; }
    public object Item { get; private set; }

    public GOSNavigationBarTree()
    {

    }
    public GOSNavigationBarTree(object item, string? caption, Action? notifier = null)
    {
        Item = item;
        Caption = caption;
        Notifier = notifier;
    }
    public void SetActionNotification(Action? notifier) => this.Notifier = notifier;
    public void SetItem(object? item, string? caption, Action? notifier = null)
    {
        Item = item;
        Caption = caption;
        Notifier = notifier;
    }
    public void SetChildren(IEnumerable? children, string? captionChild)
    {
        CaptionChildren = captionChild;
        if (Children.Count() > 0)
            Children.Clear();
        if (children is null)
            return;
        foreach (var item in children)
        {
            Children.Add(new GOSNavigationBarTree(item, captionChild));
        }
    }
    private Action? notiferChanged;
    public void SetNotifierTreeChanged(Action? notifier)
    {
        notiferChanged = notifier;
    }
    public void NotifyTreeChanged()
    {
        notiferChanged?.Invoke();
    }
    public override string? ToString()
    {
        return Item?.ToString();
    }
}
