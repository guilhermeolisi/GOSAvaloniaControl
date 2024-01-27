using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using GOSAvaloniaControls.NavigationBar.Model;
using System.Collections.ObjectModel;

namespace GOSAvaloniaControls;

public class GOSNavigationBar : TemplatedControl
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
    ObservableCollection<GOSNavigationBarTree> ChildrenItems = new();
    public GOSNavigationBar()
    {
        MainItemProperty.Changed.AddClassHandler<GOSNavigationBar>((x, e) => x.ChangeMainItem());
    }
    Button homebt, returnbt;
    TextBlock captionChildrentb;
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
        listChildren = e.NameScope.Find<ListBox>("PART_listchildren");
        listChildren.ItemsSource = ChildrenItems;
        listChildren.SelectionChanged += (s, e) => ChildSelected(listChildren.SelectedIndex);
            
        HomeCommand();
    }

    //private void ListChildren_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    //{
    //    if ()
    //}

    private void ChangeMainItem()
    {
        HomeCommand();
        toolTipHome.Content = $"Go to {MainItem?.Caption}";
    }
    bool changedLevel;
    protected void ChildSelected(int index)
    {
        if (index < 0)
            return;
        if (ChildrenItems[index].Children is not null && ChildrenItems[index].Children.Count > 0)
        {
            Indexes.Add(index);
            UpdateChildrenCaption(ChildrenItems[index].CaptionChildren);
            ChangeChildrenItems(ChildrenItems[index].Children);
            changedLevel = true;
        }
        else
        {
            if (changedLevel)
            {
                Indexes.Add(index);
                changedLevel = false;
            }
            else
            {
                if (Indexes.Count == 0)
                    Indexes.Add(index);
                else
                    Indexes[Indexes.Count - 1] = index;
            }
        }
        GOSNavigationBarTree temp = GetItemFromIndex();
        Selected = temp.Item;
        UpdateButtonsVisibility();
    }
    private void ChangeChildrenItems(List<GOSNavigationBarTree> children)
    {
        if (children is null || children.Count == 0)
            return;
        if (!ChildrenItems.Contains(children[0]))
        {
            listChildren.SelectedIndex = -1;
            ChildrenItems.Clear();
            for (int i = 0; i < children.Count; i++)
            {
                ChildrenItems.Add(children[i]);
            }
            
        }
    }
    private void UpdateButtonsVisibility()
    {
        homebt.IsEnabled = Indexes.Count > 1;
        returnbt.IsEnabled = Indexes.Count > 0;


        if (Indexes.Count > 1)
        {
            GOSNavigationBarTree temp = GetItemFromIndex();
            toolTipReturn.Content = $"Return to {temp?.Caption}";
        }
    }
    private void UpdateChildrenCaption(string caption)
    {
        captionChildrentb.Text = caption;//$"[{caption}]";
    }
    private void HomeCommand()
    {
        Indexes.Clear();
        UpdateButtonsVisibility();
        if (MainItem is null)
        {
            //ChangeChildrenItems(null);
            Selected = null;
            ChildrenItems.Clear();
            captionChildrentb.Text = string.Empty;
        }
        else
        {
            ChangeChildrenItems(MainItem?.Children!);
            UpdateChildrenCaption(MainItem?.CaptionChildren);
            listChildren.SelectedIndex = -1;
            Selected = MainItem.Item;
        }
    }
    private void ReturnCommnad()
    {
        Indexes.RemoveAt(Indexes.Count - 1);
        UpdateButtonsVisibility();
        GOSNavigationBarTree temp = GetItemFromIndex();
        if (temp?.Children?.Count > 0)
        {
            ChangeChildrenItems(temp.Children);
            UpdateChildrenCaption(temp.CaptionChildren);
        }
        changedLevel = true;
        listChildren.SelectedIndex = -1;
        Selected = temp?.Item;
    }
    private GOSNavigationBarTree GetItemFromIndex()
    {
        GOSNavigationBarTree temp = MainItem;
        for (int i = 0; i < Indexes.Count; i++)
        {
            if (temp?.Children is not null)
            {
                if (temp.Children.Count > Indexes[i])
                    temp = temp.Children[Indexes[i]];
                else
                    Indexes.RemoveAt(i);
            }
        }
        return temp;
    }
}
