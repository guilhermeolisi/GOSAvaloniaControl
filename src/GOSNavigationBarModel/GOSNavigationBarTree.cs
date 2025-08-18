using System.Collections;

namespace GOSAvaloniaControls.NavigationBar.Model;

public class GOSNavigationBarTree
{
    private TreeCaption? _treeCaption;
    private bool isSelected;
    public string? Caption { get; set; }
    public List<GOSNavigationBarTree> Children { get; private set; } = new();
    public object Item { get; private set; }
    public int TotalLevel => Children.Count == 0 ? 1 : 1 + Children.Max(c => c.TotalLevel);

    public GOSNavigationBarTree()
    {

    }
    public GOSNavigationBarTree(object item, string? caption)
    {
        Item = item;
        Caption = caption;
    }
    public void SetItem(object? item, string? caption)
    {
        Item = item;
        Caption = caption;

    }
    public void ClearTree()
    {
        Item = null!;
        Caption = null;
        Children.Clear();
        _treeCaption = null;
        isSelected = false;

        notiferTreeChanged?.Invoke(new GOSNavigationBarTree(true, null));
        notiferTreeChanged = null;
    }
    public void SetChildren(IEnumerable? children, string? captionChild)
    {
        if (Children.Count() > 0)
            Children.Clear();
        if (children is null)
            return;
        foreach (var item in children)
        {
            Children.Add(new GOSNavigationBarTree(item, captionChild));
        }
    }
    public void ProcessTreeCaption()
    {
        _treeCaption ??= new TreeCaption(this);
        if (!_treeCaption.IsEquivalent(this))
        {
            DeselectAll();
        }
        _treeCaption.UpdateTreeCaption(this);
    }
    private void DeselectAll()
    {
        isSelected = false;
        if (Children?.Count > 0)
        {
            foreach (var child in Children)
            {
                child.DeselectAll();
            }
        }
    }
    public GOSNavigationBarTree? GetSelectedTree()
    {
        if (isSelected)
            return this;
        if (Children is not null && Children.Count > 0)
        {
            foreach (var child in Children)
            {
                var selectedChild = child.GetSelectedTree();
                if (selectedChild is not null)
                    return selectedChild;
            }
        }
        return null;
    }
    public GOSNavigationBarTree? GetParentOfSelected()
    {
        GOSNavigationBarTree? result = null;

        if (Children is not null && Children.Count > 0)
        {
            foreach (var child in Children)
            {

                if (child.isSelected)
                {
                    return this; // Return this node as the parent of the selected child.
                }
                else
                {
                    result = child.GetParentOfSelected();
                    if (result is not null)
                    {
                        return result; // Return the parent of the selected child.
                    }
                }
            }
        }
        // Return null if this is the root or if no parent is found.
        return null; // No selected child found in this branch. 
    }
    public bool SelectFromTree(GOSNavigationBarTree? tree)
    {
        bool result = false;
        if (tree is null)
        {
            DeselectAll();
            result = true;
        }
        else if (tree == this)
        {
            isSelected = true;
            result = true;
        }
        else if (isSelected)
            isSelected = false; // Deselect this node if it was previously selected.

        foreach (var child in Children)
        {
            if (child is null)
            {
                continue;
            }
            if (tree == this)
            {
                child.DeselectAll(); // Deselect all children if this node is selected.
            }
            else
            {
                if (child.SelectFromTree(tree))
                {
                    result = true; // If a child was selected, return true.
                }
            }
        }
        notiferTreeChanged?.Invoke(GetSelectedTree());
        return result;
    }
    private Action<GOSNavigationBarTree?>? notiferTreeChanged;
    public void SetNotifierTreeChanged(Action<GOSNavigationBarTree?>? notifier)
    {
        if (notiferTreeChanged != notifier)
            notiferTreeChanged = notifier;
    }
    public void NotifyTreeChanged()
    {
        notiferTreeChanged?.Invoke(GetSelectedTree());
    }
    public override string? ToString()
    {
        return Item?.ToString();
    }
}
