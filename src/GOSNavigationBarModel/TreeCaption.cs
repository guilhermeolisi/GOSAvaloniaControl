

namespace GOSAvaloniaControls.NavigationBar.Model;

internal class TreeCaption
{
    string Caption;
    TreeCaption[] childen;

    public TreeCaption(GOSNavigationBarTree? barTree)
    {
        UpdateTreeCaption(barTree);
    }
    public void UpdateTreeCaption(GOSNavigationBarTree? barTree)
    {
        Caption = barTree?.Caption ?? string.Empty;
        if (barTree?.Children?.Count > 0)
        {
            if (childen is null || childen.Length != barTree.Children.Count)
                childen = new TreeCaption[barTree.Children.Count];
            for (int i = 0; i < barTree.Children.Count; i++)
            {
                childen[i] = new TreeCaption(barTree.Children[i]);
            }
        }
        else
        {
            childen = Array.Empty<TreeCaption>();
        }
    }
    public bool IsEquivalent(GOSNavigationBarTree? barTree)
    {
        if (barTree is null)
            return false;
        if (Caption != barTree.Caption)
            return false;
        if (childen.Length != barTree.Children.Count)
            return false;
        for (int i = 0; i < childen.Length; i++)
        {
            if (!childen[i].IsEquivalent(barTree.Children[i]))
                return false;
        }
        return true;
    }
}
