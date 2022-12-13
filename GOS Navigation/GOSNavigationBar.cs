using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using GOSAvaloniaControls.NavigationBar.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GOSAvaloniaControls;

public /*partial*/ class GOSNavigationBar : TemplatedControl
{
    public static readonly StyledProperty<GOSNavigationBarTree?> MainItemProperty = AvaloniaProperty.Register<GOSNavigationBar, GOSNavigationBarTree?>(nameof(MainItem));
    public static readonly StyledProperty<object> SelectedProperty = AvaloniaProperty.Register<GOSNavigationBar, object>(nameof(Selected), false, false, BindingMode.TwoWay);


    public GOSNavigationBarTree? MainItem
    {
        get => GetValue(MainItemProperty);
        set => SetValue(MainItemProperty, value);
    }
    public object Selected
    {
        get => GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }


    List<int> Indexes = new();
    List<GOSNavigationBarTree> PathItems;
    List<GOSNavigationBarTree> ChildrenItems = new();
    public GOSNavigationBar()
    {
        MainItemProperty.Changed.AddClassHandler<GOSNavigationBar>((x, e) => x.ChangeMainItem());
    }
    Button homebt, returnbt;
    TextBlock captionChildrentb;
    ListBox listPath;
    ListBox listChildren;
    ToolTip toolTipHome = new();
    ToolTip toolTipReturn = new();
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        homebt = e.NameScope.Find<Button>("PART_home");
        homebt.Click += (s, e) => HomeCommand();
        ToolTip.SetTip(toolTipHome, homebt);
        returnbt = e.NameScope.Find<Button>("PART_return");
        returnbt.Click += (s, e) => ReturnCommnad();
        ToolTip.SetTip(toolTipReturn, returnbt);
        captionChildrentb = e.NameScope.Find<TextBlock>("PART_captionchildren");
        listPath = e.NameScope.Find<ListBox>("PART_listpath");
        listChildren = e.NameScope.Find<ListBox>("PART_listchildren");
        listChildren.Items = ChildrenItems;
        //listChildren.PropertyChanged += ListChildren_PropertyChanged;
        listChildren.GetObservable(ListBox.SelectedIndexProperty)
            .Subscribe(x => ChildSelected(x));

    }

    //private void ListChildren_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    //{
    //    if ()
    //}

    private void ChangeMainItem()
    {
        toolTipHome.Content = $"Go to {MainItem?.Caption}";
    }
    private void ChildSelected(int index)
    {
        if (index < 0)
            return;
        if (ChildrenItems[index].Children is not null && ChildrenItems[index].Children.Count > 0)
        {
            Indexes.Add(index);
            ChangeChildren(ChildrenItems[index].Children);
            UpdateChildrenCaption(ChildrenItems[index].CaptionChild);
        }
        Selected = ChildrenItems[index].Item;
        UpdateButtonsVisibility();
    }
    private void ChangeChildren(List<GOSNavigationBarTree> children)
    {
        if (children is null || children.Count == 0)
            return;
        if (!ChildrenItems.Contains(children[0]))
        {
            ChildrenItems.Clear();
            ChildrenItems.AddRange(children);
        }
    }
    private void UpdateButtonsVisibility()
    {
        homebt.IsVisible = Indexes.Count > 0;
        returnbt.IsVisible = Indexes.Count > 1;


        if (Indexes.Count > 1)
        {
            GOSNavigationBarTree temp = MainItem;
            for (int i = 0; i < Indexes.Count; i++)
            {
                if (temp?.Children is not null)
                    temp = temp.Children[i];
            }

            toolTipReturn.Content = $"Return to {temp?.Caption}";
        }
    }
    private void UpdateChildrenCaption(string caption)
    {
        captionChildrentb.Text = $"[{caption}]";
    }
    private void HomeCommand()
    {
        Indexes.Clear();
        ChangeChildren(MainItem?.Children!);
        UpdateChildrenCaption(MainItem.CaptionChild);
        Selected = MainItem.Item;
        UpdateButtonsVisibility();
    }
    private void ReturnCommnad()
    {
        Indexes.RemoveAt(Indexes.Count - 1);
        GOSNavigationBarTree temp = MainItem;
        for (int i = 0; i < Indexes.Count; i++)
        {
            if (temp?.Children is not null)
                temp = temp.Children[i];
        }
        if (temp?.Children?.Count > 0)
        {
            ChangeChildren(temp.Children);
            UpdateChildrenCaption(temp.CaptionChild);
        }
        Selected = temp.Item;
        UpdateButtonsVisibility();
    }
}
