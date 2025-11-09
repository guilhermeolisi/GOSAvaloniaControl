using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Svg.Skia;
using Avalonia.Threading;

namespace GOSAvaloniaControls;

public partial class GOSImageViewer
{
    //private Bitmap? ImageToView;
    private IImage? ImageToView;
    private Image _imageControl;
    private ZoomBorder? _zoomBorder;
    private Dispatcher UIDispatcher = Dispatcher.UIThread;
    private async void ChangeFile()
    {
        await GetImageFromFile();
        await SetSourceToImageControl();
    }
    bool isGettingImage = false;
    private async Task GetImageFromFile()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            return;
        if (!File.Exists(FilePath))
        {

        }
        isGettingImage = true;

        bool isSVG = Path.GetExtension(FilePath).Equals(".svg", StringComparison.OrdinalIgnoreCase);
        if (isSVG)
        {
            await using (var imageStream = File.OpenRead(FilePath))
            {
                //SvgSource svgSource = SvgSource.Load(FilePath);
                SvgSource svgSource = await Task.Run(() => SvgSource.LoadFromStream(imageStream));
                var svgImage = new SvgImage
                {
                    Source = svgSource
                };
                ImageToView = svgImage;
            }
        }
        else
        {
            try
            {
                await using (var imageStream = File.OpenRead(FilePath))
                {
                    //ImageToView = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
                    ImageToView = await Task.Run(() => new Bitmap(imageStream));
                }
            }
            catch (Exception e)
            {

            }
        }
        isGettingImage = false;
    }
    bool isImageControlNull = false;
    private async Task SetSourceToImageControl()
    {
        if (_imageControl is null)
        {
            isImageControlNull = true;
            return;
        }
        isImageControlNull = false;

        while (isGettingImage)
        {
            //Thread.Sleep(50);
            await Task.Delay(50);
        }

        UIDispatcher.Post(() =>
        {
            _imageControl.Source = ImageToView;
        }, DispatcherPriority.ApplicationIdle);
    }
}