using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using GOSAvaloniaControls.NavigationBar.Model;
using GOSNavigationBar;
using System.Collections.ObjectModel;

namespace GOSAvaloniaControls;
[TemplatePart(PART_ElementReturnButton, typeof(Button))]
[TemplatePart(PART_ElementHomeButton, typeof(Button))]
[TemplatePart(PART_ElementCaptionChildren, typeof(TextBlock))]
[TemplatePart(PART_ElementListChildren, typeof(ListBox))]
public class GOSNavigationBar : TemplatedControl
{
    private const string PART_ElementReturnButton = "PART_ReturnButton";
    private const string PART_ElementHomeButton = "PART_HomeButton";
    private const string PART_ElementCaptionChildren = "PART_CaptionChildren";
    private const string PART_ElementListChildren = "PART_ListChildren";


    public static readonly StyledProperty<GOSNavigationBarTree?> MainItemProperty = AvaloniaProperty.Register<GOSNavigationBar, GOSNavigationBarTree?>(nameof(MainItem), defaultBindingMode: BindingMode.OneWay);
    public static readonly StyledProperty<object?> SelectedProperty = AvaloniaProperty.Register<GOSNavigationBar, object?>(nameof(Selected), null, false, BindingMode.TwoWay);


    public GOSNavigationBarTree? MainItem
    {
        get => GetValue(MainItemProperty);
        set => SetValue(MainItemProperty, value);
    }
    public object? Selected
    {
        get => GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }


    List<int> Indexes = new();
    ObservableCollection<GOSNavigationBarTree> ChildrenItems = new();
    public GOSNavigationBar()
    {
        MainItemProperty.Changed.AddClassHandler<GOSNavigationBar>((x, e) => x.ChangeMainItem(e));
    }
    internal Button? _homeButton, _returnButton;
    private ListBox? _listChildren;
    internal TextBlock? CaptionChildrentb { get; set; }
    internal ListBox? ListChildren
    {
        get => _listChildren;
        private set
        {
            if (_listChildren != null)
            {
                _listChildren.ItemsSource = null;
                _listChildren.SelectionChanged -= ListChildren_SelectionChanged;
            }

            _listChildren = value;

            if (_listChildren != null)
            {
                _listChildren.ItemsSource = ChildrenItems;
                _listChildren.SelectionChanged += ListChildren_SelectionChanged;
            }
        }
    }

    private void ListChildren_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ChildSelected(ListChildren.SelectedIndex);
    }

    ToolTip toolTipHome = new();
    ToolTip toolTipReturn = new();

    internal Button? HomeButton
    {
        get => _homeButton;
        private set
        {
            if (_homeButton != null)
                _homeButton.Click -= Homebt_Click;

            _homeButton = value;

            if (_homeButton != null)
            {
                _homeButton.Click += Homebt_Click;
                ToolTip.SetTip(_homeButton, toolTipHome);
            }
#if DEBUG
            else
            {

            }
#endif

        }
    }

    private void Homebt_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        HomeCommand();
    }

    internal Button? ReturnButton
    {
        get => _returnButton;
        private set
        {
            if (_returnButton != null)
                _returnButton.Click -= ReturnButton_Click;

            _returnButton = value;

            if (_returnButton != null)
            {
                _returnButton.Click += ReturnButton_Click;
                ToolTip.SetTip(_returnButton, toolTipReturn);
            }
#if DEBUG
            else
            {

            }
#endif
        }
    }

    private void ReturnButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ReturnCommnad();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        HomeButton = e.NameScope.Find<Button>(PART_ElementHomeButton);
        ReturnButton = e.NameScope.Find<Button>(PART_ElementReturnButton);

        CaptionChildrentb = e.NameScope.Find<TextBlock>(PART_ElementCaptionChildren);
        ListChildren = e.NameScope.Find<ListBox>(PART_ElementListChildren);

        //HomeCommand();
    }
    TreeCaption treeCaption = new TreeCaption(null);
    private void notifyTreeChanged()
    {
        if (!treeCaption.IsEquivalent(MainItem))
        {
            HomeCommand();
            treeCaption.UpdateTreeCaption(MainItem);
        }
        else
        {
            // Para forçar atualização da UI
            GOSNavigationBarTree temp = GetItemFromIndex();
            Selected = temp.Item;
        }
    }
    private void ChangeMainItem(AvaloniaPropertyChangedEventArgs e)
    {
        var newValue = e.GetNewValue<GOSNavigationBarTree?>();
        newValue?.SetNotifierTreeChanged(notifyTreeChanged);
        //HomeCommand();
        toolTipHome.Content = MainItem is null ? string.Empty : $"Go to {MainItem.Caption}";
        treeCaption.UpdateTreeCaption(newValue);
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
        if (ListChildren is null)
            return;
        if (!ChildrenItems.Contains(children[0]))
        {
            ListChildren.SelectedIndex = -1;
            ChildrenItems.Clear();
            for (int i = 0; i < children.Count; i++)
            {
                ChildrenItems.Add(children[i]);
            }

        }
    }
    private void UpdateButtonsVisibility()
    {

        if (_homeButton is null || _returnButton is null)
            return;

        _homeButton.IsEnabled = Indexes.Count > 0;
        _returnButton.IsEnabled = Indexes.Count > 1;


        if (Indexes.Count > 1)
        {
            GOSNavigationBarTree temp = GetItemFromIndexLevel(Indexes.Count - 2); // -1 pelo indice ser 0 based e -1 para retornar um nivel
            toolTipReturn.Content = temp is null ? string.Empty : $"Return to {temp.Caption}";
        }
    }
    private void UpdateChildrenCaption(string caption)
    {
        if (CaptionChildrentb is null)
            return;
        CaptionChildrentb.Text = caption;//$"[{caption}]";
    }
    private void HomeCommand()
    {
        if (ListChildren is null)
            return;

        Indexes.Clear();
        UpdateButtonsVisibility();
        if (MainItem is null)
        {
            //ChangeChildrenItems(null);
            Selected = null;
            ChildrenItems.Clear();
            CaptionChildrentb.Text = string.Empty;
        }
        else
        {
            ChangeChildrenItems(MainItem.Children!);
            UpdateChildrenCaption(MainItem.CaptionChildren);
            ListChildren.SelectedIndex = -1;
#if DEBUG
            if (MainItem.Item == Selected)
            { }
#endif
            if (Selected is not null)
                Selected = null;
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
        ListChildren.SelectedIndex = -1;
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
    private GOSNavigationBarTree GetItemFromIndexLevel(int level)
    {
        if (level > Indexes.Count - 1)
            return null;
        GOSNavigationBarTree temp = MainItem;
        for (int i = 0; i <= level; i++)
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
