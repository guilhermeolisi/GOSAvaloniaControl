using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace GOSAvaloniaControls;

public partial class GOSImageViewer
{
    private Bitmap? ImageToView;
    private Image _imageControl;
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
        }, DispatcherPriority.Layout);
    }
}