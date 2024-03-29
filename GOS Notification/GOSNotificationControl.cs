using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Collections.ObjectModel;

namespace GOSAvaloniaControls;

public class GOSNotificationControl : TemplatedControl, IGOSNotification
{
    private Thread mainThread = Thread.CurrentThread;
    SynchronizationContext? UIContext = SynchronizationContext.Current; //Do AvaloniaEditDocumentBusiness.cs

    private DispatcherTimer timerBallon = new();
    private Button buttonBell;
    private ItemsControl showNotifications;
    private Flyout flyout;
    private FlyoutBase flyoutBallon;
    FluentAvalonia.UI.Controls.InfoBadge infoBadge;

    public static readonly StyledProperty<ObservableCollection<NotificationItem>> ItemsProperty = AvaloniaProperty.Register<GOSNotificationControl, ObservableCollection<NotificationItem>>(nameof(Items), new ObservableCollection<NotificationItem>(), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<double> SizeBellProperty = AvaloniaProperty.Register<GOSNotificationControl, double>(nameof(Items), 30, defaultBindingMode: BindingMode.OneWay);

    public ObservableCollection<NotificationItem> Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }
    public double SizeBell
    {
        get => GetValue(SizeBellProperty);
        set => SetValue(SizeBellProperty, value);
    }

    ObservableCollection<BallonItem> ItemsBallon = new();

    public GOSNotificationControl()
    {
        timerBallon.Interval = TimeSpan.FromSeconds(1);
        timerBallon.Tick += TimerBallonTick;
        ItemsProperty.Changed.AddClassHandler<GOSNotificationControl>((x, e) => x.ItemsPropertyChanged());
        SizeBellProperty.Changed.AddClassHandler<GOSNotificationControl>((x, e) => x.SizeBellPropertyChanged(x.SizeBell));
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var ballons = e.NameScope.Find<ItemsControl>("PART_ballon");
        //ballons.Items = ItemsBallon;
        ballons.ItemsSource = ItemsBallon;

        infoBadge = e.NameScope.Find<FluentAvalonia.UI.Controls.InfoBadge>("PART_infoBadge");
        //flyout = e.NameScope.Find<Flyout>("PART_flyout");


        buttonBell = e.NameScope.Find<Button>("PART_buttonBell");
        flyout = buttonBell.Flyout as Flyout;
        flyoutBallon = FlyoutBase.GetAttachedFlyout(buttonBell);//.At//e.NameScope.Find<Flyout>("PART_flyoutBallon");
        //flyoutBallon.ShowMode = FlyoutShowMode.Transient;
        flyout.ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway;

        buttonBell.IsEnabled = Items is not null ? Items.Count > 0 : false;
        buttonBell.Click += (_, _) =>
        {
            infoBadge.IsVisible = false;
            countNotification = 0;
        };
        if (SizeBell != 0)
        {
            buttonBell.Height = SizeBell;
            buttonBell.Width = SizeBell;
        }
        showNotifications = e.NameScope.Find<ItemsControl>("PART_buttonBellFlyout");
        if (Items is not null)
        {
            Items.CollectionChanged += Items_CollectionChanged;
            //if (showNotifications.Items != Items)
            //{
            //    showNotifications.Items = Items;
            //}
            if (showNotifications.ItemsSource != Items)
            {
                showNotifications.ItemsSource = Items;
            }
        }
        showNotifications.PointerPressed += Notification_PointerPressed;


        //https://github.com/AvaloniaUI/Avalonia/issues/4616
        //http://reference.avaloniaui.net/api/Avalonia.Controls/ResourceDictionary/50FEA02D
        //if (Application.Current.TryFindResource(""))
        //{
        //    wrap[ForegroundProperty] = new DynamicResourceExtension("MyResource");
        //}
    }
    void SizeBellPropertyChanged(double newSize)
    {
        if (buttonBell is null)
            return;
        buttonBell.Height = newSize;
        buttonBell.Width = newSize;
    }
    void ItemsPropertyChanged()
    {
        if (Items is null)
            return;

        Items.CollectionChanged += Items_CollectionChanged;
    }
    private const int intervalMiliseconds = 5000;
    private void TimerBallonTick(object? sender, EventArgs e)
    {
        timerBallon.Stop();

        for (int i = 0; i < ItemsBallon.Count; i++)
        {
            if (ItemsBallon[i].Time + TimeSpan.FromMilliseconds(intervalMiliseconds - 50) <= DateTime.Now)
            {
#if DEBUG
                var trash3 = DateTime.Now;
                var trash = intervalMiliseconds - (DateTime.Now - ItemsBallon[0].Time).TotalMilliseconds;
                var trash2 = ItemsBallon[0].Time;
                var trash4 = (DateTime.Now - ItemsBallon[0].Time).TotalMilliseconds;
#endif
                ItemsBallon.RemoveAt(i);
                i--;
            }
        }
        if (ItemsBallon.Count > 0)
        {
            var time = ItemsBallon.Min(x => x.Time);

            timerBallon.Stop();
#if DEBUG
            var trash = intervalMiliseconds - (DateTime.Now - ItemsBallon[0].Time).TotalMilliseconds;
#endif


            timerBallon.Interval = TimeSpan.FromMilliseconds(intervalMiliseconds - (DateTime.Now - ItemsBallon[0].Time).TotalMilliseconds);
            timerBallon.Start();
            if (!flyoutBallon.IsOpen)
            {
                flyoutBallon.ShowAt(buttonBell);
            }
        }
        else
        {
            flyoutBallon.Hide();
        }
    }
    int countNotification = 0;
    private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            bool startTimer = false;
            for (int i = 0; i < e.NewItems?.Count; i++)
            {
                if (e.NewItems[i] is not null && e.NewItems[i] is NotificationItem item && item.ShowBallon)
                {
                    ItemsBallon!.Add(new(item, DateTime.Now));
                    if (!startTimer)
                        startTimer = true;
                }
            }
            if (startTimer)
            {
                if (!timerBallon.IsEnabled)
                {
                    timerBallon.Stop();
                    timerBallon.Interval = TimeSpan.FromMilliseconds(intervalMiliseconds);
                    timerBallon.Start();
                }
            }
            countNotification++;

        }
        else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
            for (int i = 0; i < e.OldItems?.Count; i++)
            {
                for (int j = 0; j < ItemsBallon!.Count; j++)
                {
                    if (ItemsBallon[i].Item == e.OldItems[i])
                    {
                        ItemsBallon!.RemoveAt(i);
                    }
                }
            }
        }
        if (ItemsBallon.Count > 0)
        {
            //FlyoutBase.ShowAttachedFlyout(buttonBell);
            flyoutBallon.ShowAt(buttonBell);
        }
        if (Items?.Count > 0 && !buttonBell.IsEnabled)
            buttonBell.IsEnabled = true;
        else if (Items?.Count == 0 && buttonBell.IsEnabled)
            buttonBell.IsEnabled = false;
        infoBadge.Value = countNotification;
        if (Items?.Count > 0 && !infoBadge.IsVisible && !flyout.IsOpen)
            infoBadge.IsVisible = true;
    }

    public void AddNotification(byte severity, string message, bool showBallon)
    {

        if (Thread.CurrentThread == mainThread)
        {
            localMethod(severity, message, showBallon);
        }
        else
        {
            UIContext?.Post(_ =>
            {
                localMethod(severity, message, showBallon);
            }, null);
        }
        void localMethod(byte severity, string message, bool showBallon)
        {
            if (Items is null)
            {
                Items = new();
            }
            //if (showNotifications.Items is null || showNotifications.Items != Items)
            //    showNotifications.Items = Items;
            if (showNotifications.ItemsSource is null || showNotifications.ItemsSource != Items)
            {
                showNotifications.ItemsSource = Items;
            }
            Items.Add(new NotificationItem(severity, message, showBallon));
        }
        //if (Items is null)
        //{
        //    Items = new();
        //}
        //if (showNotifications.Items is null || showNotifications.Items != Items)
        //    showNotifications.Items = Items;
        //Items.Add(new NotificationItem(severity, message, showBallon));
    }
    public bool UIContextIsNull => UIContext is null;
    public void SetUIContext(SynchronizationContext? uiContext) => UIContext = uiContext;
    public static void Notification_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {

        Visual? visual = e.Source as Visual;
        if (visual is null)
            return;

        FluentAvalonia.UI.Controls.InfoBar? info = visual.FindAncestorOfType<FluentAvalonia.UI.Controls.InfoBar>();

        if (info is null)
            return;

        if (info.MaxHeight != 80)
        {
            info.MaxHeight = 80;
        }
        else
        {
            info.MaxHeight = double.PositiveInfinity;
        }
    }
}
