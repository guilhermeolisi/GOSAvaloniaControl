using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Controls.Primitives;

namespace GOSAvaloniaControls;

public partial class GOSImageViewer : TemplatedControl
{
    public static readonly StyledProperty<string?> FilePathProperty = AvaloniaProperty.Register<GOSImageViewer, string?>(nameof(FilePath));

    public string? FilePath
    {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }
    public GOSImageViewer()
    {
        FilePathProperty.Changed.AddClassHandler<GOSImageViewer>((x, e) => x.ChangeFile());
    }
    protected override async void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _imageControl = e.NameScope.Find<Image>("PART_Image");
        _zoomBorder = e.NameScope.Find<ZoomBorder>("PART_ZoomBorder");
        //https://github.com/wieslawsoltes/PanAndZoom
        //_zoomBorder.EnableDoubleClickZoom = true;
        //_zoomBorder.EnableAnimations = true;
        //_zoomBorder.BoundsMode = ;
        //_zoomBorder.Whe
        //_zoomBorder.AutoCalculateMinZoom = true;
        _zoomBorder!.DoubleTapped += (sender, tappedEA) =>
        {

            if (_zoomBorder.ZoomX == 1.0 && _zoomBorder.ZoomY == 1.0)
            {
                //var point = tappedEA.GetCurrentPoint(sender as Control);
                var point = tappedEA.GetPosition(_zoomBorder);
                var x = point.X;
                var y = point.Y;
                _zoomBorder.ZoomTo(3.0, x, y);
            }
            else
            {
                _zoomBorder.ResetMatrix();
            }
        };
        if (isImageControlNull)
            await SetSourceToImageControl();
    }
}
