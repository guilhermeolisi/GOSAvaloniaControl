using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using GOSAvaloniaControls.NavigationBar.Model;
using System.Collections.ObjectModel;

namespace GOSAvaloniaControls;

public class GOSNavigationBarDesign : GOSNavigationBar
{
    public GOSNavigationBarTree Items { get; set; }
    public GOSNavigationBarDesign()
    {
        GOSNavigationBarTree item = new("Teste", "Teste Caption");
        item.SetChildren(new List<string>()
        {
            "Child 1",
            "Child 2",
        }
        , "Caption Children 1");
        for (int i = 0; i < item.Children.Count; i++)
        {
            item.Children[i].SetChildren(new List<string>()
            {
                $"Child {i}-1",
                $"Child {i}-2",
                $"Child {i}-3"
            },
            "Caption Children 2");
        }
        Items = item;
        //MainItem = item;
        this.DataContext = this;
        this.Bind(GOSNavigationBar.MainItemProperty, new Binding("Items"));
        //ChildSelected(1);
        //ChildSelected(1);
    }
}
