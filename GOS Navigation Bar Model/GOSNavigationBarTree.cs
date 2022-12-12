using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSAvaloniaControls.NavigationBar.Model;

public class GOSNavigationBarTree
{
    public string Caption { get; set; }
    public string CaptionChild { get; set; }
    public List<GOSNavigationBarTree> Children { get; private set; } = new();//List<GOSNavigationBarTree>();
    public Action? Notifier { get; private set; }
    public object Item { get; private set; }

    public GOSNavigationBarTree()
    {

    }
    public GOSNavigationBarTree(object item, string caption, Action? notifier = null/*, IEnumerable? children = null, string? captionChild = null*/)
    {
        Item = item;
        Caption = caption;
        Notifier = notifier;
        //if (children != null)
        //    SetChildren(children, captionChild);
    }
    public void SetActionNotification(Action? notifier) => this.Notifier = notifier;
    public void SetChildren(IEnumerable children, string captionChild)
    {
        CaptionChild = captionChild;
        if (Children.Count() > 0)
            Children.Clear();
        foreach (var item in children)
        {
            Children.Add(new GOSNavigationBarTree(item, captionChild));
        }
    }
    public override string ToString()
    {
        return Item?.ToString();
    }
}
