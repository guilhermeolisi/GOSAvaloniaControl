using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using GOSAvaloniaControls.NavigationBar.Model;
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


    public static readonly StyledProperty<GOSNavigationBarTree?> RootItemProperty = AvaloniaProperty.Register<GOSNavigationBar, GOSNavigationBarTree?>(nameof(RootItem), defaultBindingMode: BindingMode.OneWay);
    public static readonly StyledProperty<object?> SelectedProperty = AvaloniaProperty.Register<GOSNavigationBar, object?>(nameof(Selected), null, false, BindingMode.TwoWay);


    public GOSNavigationBarTree? RootItem
    {
        get => GetValue(RootItemProperty);
        set => SetValue(RootItemProperty, value);
    }
    public object? Selected
    {
        get => GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }


    //List<int> Indexes = new();
    ObservableCollection<GOSNavigationBarTree> ChildrenItems = new();
    public GOSNavigationBar()
    {
        RootItemProperty.Changed.AddClassHandler<GOSNavigationBar>((x, e) => x.ChangeRootItem(e));
    }
    internal Button? _homeButton, _returnButton;
    private ListBox? _listChildren;
    internal TextBlock? CaptionChildrentb { get; set; }
    internal ListBox? ListChildrenlb
    {
        get => _listChildren;
        private set
        {
            if (_listChildren is not null)
            {
                _listChildren.SelectionChanged -= ListChildren_SelectionChanged;
                _listChildren.ItemsSource = null;
            }

            _listChildren = value;

            if (_listChildren is not null)
            {
                _listChildren.ItemsSource = ChildrenItems;
                _listChildren.SelectionChanged += ListChildren_SelectionChanged;
            }
        }
    }

    private void ListChildren_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ChildSelected(ListChildrenlb.SelectedIndex);
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
        ListChildrenlb = e.NameScope.Find<ListBox>(PART_ElementListChildren);
        if (RootItem is not null && RootItem.Item is not null)
            RootItem.SetNotifierTreeChanged(notifyChangedFromTree);
        if (RootItem is not null && RootItem.Item is not null)
            notifyChangedFromTree(RootItem.GetSelectedTree());
        //HomeCommand();
    }
    //protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    //{
    //    base.OnAttachedToLogicalTree(e);
    //}
    //protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    //{
    //    base.OnAttachedToVisualTree(e);
    //}
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        if (RootItem is not null)
            RootItem.SetNotifierTreeChanged(null);
    }
    //protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    //{
    //    base.OnDetachedFromVisualTree(e);
    //}
    private void notifyChangedFromTree(GOSNavigationBarTree? selected)
    {
        if (selected?.Item is bool)
        {
            ChangeSelectedFromTree(null);
            return;
        }
        ChangeSelectedFromTree(selected ?? RootItem!);
    }
    private void ChangeRootItem(AvaloniaPropertyChangedEventArgs e)
    {
        var newValue = e.GetNewValue<GOSNavigationBarTree?>();
        newValue?.SetNotifierTreeChanged(notifyChangedFromTree);
        //HomeCommand();
        toolTipHome.Content = RootItem is null ? string.Empty : $"Go to {RootItem.Caption}";
        //if (CaptionChildrentb is not null && IsEffectivelyVisible && newValue is not null)
        //    notifyTreeChanged();
    }
    protected void ChildSelected(int index)
    {
        if (index < 0)
            return;
        GOSNavigationBarTree selected = ChildrenItems[index];
        RootItem?.SelectFromTree(selected);

        //ChangeSelected(selected);
        //if (ChildrenItems[index].Children is not null && ChildrenItems[index].Children.Count > 0)
        //{
        //    lock (Indexes)
        //    {
        //        Indexes.Add(index);
        //    }
        //    UpdateChildrenCaption(ChildrenItems[index].CaptionChildren);
        //    ChangeChildrenItems(ChildrenItems[index].Children);
        //    changedLevel = true;
        //}
        //else
        //{
        //    if (changedLevel)
        //    {
        //        lock (Indexes)
        //        {
        //            Indexes.Add(index);
        //        }
        //        changedLevel = false;
        //    }
        //    else
        //    {
        //        lock (Indexes)
        //        {
        //            if (Indexes.Count == 0)
        //                Indexes.Add(index);
        //            else
        //                Indexes[Indexes.Count - 1] = index;
        //        }
        //    }
        //}
        //GOSNavigationBarTree temp = GetItemFromIndex();
        //Selected = temp.Item;
        //UpdateButtonsVisibility();
    }
    private void ChangeSelectedFromTree(GOSNavigationBarTree? selected)
    {
        if (selected is null)
        {
            UpdateChildrenCaption(null);
            ChangeChildrenItems(null);
        }
        else if (selected.Children?.Count > 0)
        {
            UpdateChildrenCaption(selected!.Children[0].Caption);
            ChangeChildrenItems(selected.Children);
            if (ListChildrenlb.SelectedItem is not null)
                ListChildrenlb.SelectedItem = null;
        }
        else
        {
            // tem que verificar se está com os filhos e captions adequados. Isso é necessário por causa do momento em que o controle é criado novamente na View com um viewmodel com um caminho estabelecido.
            if (ChildrenItems is null || ChildrenItems.Count == 0)
            {
                GOSNavigationBarTree? temp = RootItem?.GetParentOfSelected();
                UpdateChildrenCaption(temp?.Children[0].Caption ?? RootItem.Children[0].Caption);
                ChangeChildrenItems(temp?.Children ?? RootItem?.Children);
                if (ListChildrenlb is not null)
                    ListChildrenlb.SelectedItem = selected;
            }


            //if (changedLevel)
            //{
            //    lock (Indexes)
            //    {
            //        Indexes.Add(index);
            //    }
            //    changedLevel = false;
            //}
            //else
            //{
            //    lock (Indexes)
            //    {
            //        if (Indexes.Count == 0)
            //            Indexes.Add(index);
            //        else
            //            Indexes[Indexes.Count - 1] = index;
            //    }
            //}
        }
        Selected = selected?.Item;
        UpdateButtonsVisibility();
    }
    private void ChangeChildrenItems(List<GOSNavigationBarTree>? children)
    {
        if (ListChildrenlb is null)
            return;
        if (children is null || children.Count == 0)
        {
            ChildrenItems.Clear();
        }
        else if (!ChildrenItems.Contains(children[0]))
        {
            ListChildrenlb.SelectedIndex = -1;
            ChildrenItems.Clear();
            for (int i = 0; i < children.Count; i++)
            {
                ChildrenItems.Add(children[i]);
            }

        }
        else
        {

        }
    }
    private void UpdateButtonsVisibility()
    {

        if (_homeButton is null || _returnButton is null)
            return;

        _homeButton.IsEnabled = RootItem is not null && Selected is not null && Selected != RootItem.Item;
        _returnButton.IsEnabled = RootItem is not null && Selected is not null && Selected != RootItem.Item;
        GOSNavigationBarTree? temp = RootItem?.GetSelectedTree();
        toolTipReturn.Content = temp is null ? string.Empty : $"Return to {temp.Caption}";

        //_homeButton.IsEnabled = Indexes.Count > 0;
        ////_returnButton.IsEnabled = Indexes.Count > 1;
        //_returnButton.IsEnabled = Indexes.Count > 0;


        //if (Indexes.Count > 1)
        //{
        //    GOSNavigationBarTree temp = GetItemFromIndexLevel(Indexes.Count - 2); // -1 pelo indice ser 0 based e -1 para retornar um nivel
        //    toolTipReturn.Content = temp is null ? string.Empty : $"Return to {temp.Caption}";
        //}
    }
    private void UpdateChildrenCaption(string? caption)
    {
        if (CaptionChildrentb is null)
            return;
        CaptionChildrentb.Text = caption ?? "No associated data";
    }
    private void HomeCommand()
    {
        RootItem?.SelectFromTree(RootItem);

        //        if (ListChildren is null)
        //            return;
        //        lock (Indexes)
        //        {
        //            Indexes.Clear();
        //        }
        //        UpdateButtonsVisibility();
        //        if (RootItem is null)
        //        {
        //            //ChangeChildrenItems(null);
        //            Selected = null;
        //            ChildrenItems.Clear();
        //            CaptionChildrentb.Text = string.Empty;
        //        }
        //        else
        //        {
        //            ChangeChildrenItems(RootItem.Children!);
        //            UpdateChildrenCaption(RootItem.CaptionChildren);
        //            ListChildren.SelectedIndex = -1;
        //#if DEBUG
        //            if (RootItem.Item == Selected)
        //            { }
        //#endif
        //            if (Selected is not null)
        //                Selected = null;
        //            Selected = RootItem.Item;
        //        }
    }
    private void ReturnCommnad()
    {
        GOSNavigationBarTree? parent = RootItem?.GetParentOfSelected();
        RootItem?.SelectFromTree(parent);

        //lock (Indexes)
        //{
        //    Indexes.RemoveAt(Indexes.Count - 1);
        //}
        //UpdateButtonsVisibility();
        //GOSNavigationBarTree temp = GetItemFromIndex();
        //if (temp?.Children?.Count > 0)
        //{
        //    ChangeChildrenItems(temp.Children);
        //    UpdateChildrenCaption(temp.CaptionChildren);
        //}
        //changedLevel = true;
        //ListChildren.SelectedIndex = -1;
        //Selected = temp?.Item;
    }

    //private GOSNavigationBarTree GetItemFromIndex()
    //{
    //    GOSNavigationBarTree temp = RootItem;
    //    lock (Indexes)
    //    {
    //        for (int i = 0; i < Indexes.Count; i++)
    //        {
    //            if (temp?.Children is not null)
    //            {
    //                if (temp.Children.Count > Indexes[i])
    //                    temp = temp.Children[Indexes[i]];
    //                else
    //                    Indexes.RemoveAt(i);
    //            }
    //        }
    //    }
    //    return temp;
    //}
    //private GOSNavigationBarTree GetItemFromIndexLevel(int level)
    //{
    //    if (level > Indexes.Count - 1)
    //        return null;
    //    GOSNavigationBarTree temp = RootItem;
    //    lock (Indexes)
    //    {
    //        for (int i = 0; i <= level; i++)
    //        {
    //            if (temp?.Children is not null)
    //            {
    //                if (temp.Children.Count > Indexes[i])
    //                    temp = temp.Children[Indexes[i]];
    //                else
    //                    Indexes.RemoveAt(i);
    //            }
    //        }
    //    }
    //    return temp;
    //}
}
