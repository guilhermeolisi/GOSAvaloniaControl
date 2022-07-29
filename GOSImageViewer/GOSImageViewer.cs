using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        if (isImageControlNull)
            await SetSourceToImageControl();
    }
}
