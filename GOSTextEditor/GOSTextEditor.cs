using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using BaseLibrary;
using System.Threading;
using System.Threading.Tasks;

namespace GOSAvaloniaControls;

public partial class GOSTextEditor : TemplatedControl
{
    //public static readonly StyledProperty<TextDocument?> DocumentProperty = AvaloniaProperty.Register<TextEditorTemplated, TextDocument?>(nameof(Document));
    //public static readonly StyledProperty<TextEditorOptions?> EditorOptionsProperty = AvaloniaProperty.Register<TextEditorTemplated, TextEditorOptions?>(nameof(EditorOptions));
    public static readonly StyledProperty<string?> FileProperty = AvaloniaProperty.Register<GOSTextEditor, string?>(nameof(File));
    //public static readonly DirectProperty<TextEditorTemplated, string?> FileProperty =        AvaloniaProperty.RegisterDirect<TextEditorTemplated, string?>(nameof(File), o => o.File, (o, v) => o.File = v);
    public static readonly StyledProperty<string?> ExtensionProperty = AvaloniaProperty.Register<GOSTextEditor, string?>(nameof(File));
    public static readonly StyledProperty<bool> IsEditingProperty = AvaloniaProperty.Register<GOSTextEditor, bool>(nameof(IsEditing), true, false, BindingMode.TwoWay);
    public static readonly DirectProperty<GOSTextEditor, bool> IsReadOnlyProperty = AvaloniaProperty.RegisterDirect<GOSTextEditor, bool>(nameof(IsReadOnly), o => o.IsReadOnly);
    public static readonly StyledProperty<bool> IsWrapProperty = AvaloniaProperty.Register<GOSTextEditor, bool>(nameof(IsWrap), true, false, BindingMode.TwoWay);
    public static readonly StyledProperty<bool> ShowToolBarProperty = AvaloniaProperty.Register<GOSTextEditor, bool>(nameof(ShowToolBar), true, false, BindingMode.OneWay);

    private string? _file;
    public string? File
    {
        get => GetValue(FileProperty);
        set => SetValue(FileProperty, value);
    }
    //public string? File
    //{
    //    get => _file;
    //    set => SetAndRaise(FileProperty, ref _file, value);
    //}
    public string? Extension
    {
        get => GetValue(ExtensionProperty);
        set => SetValue(ExtensionProperty, value);
    }
    public bool IsEditing
    {
        get => GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }
    bool _isReadOnly;
    internal bool IsReadOnly
    {
        get => _isReadOnly;
        set => SetAndRaise(IsReadOnlyProperty, ref _isReadOnly, value);
    }
    public bool IsWrap
    {
        get => GetValue(IsWrapProperty);
        set => SetValue(IsWrapProperty, value);
    }
    public bool ShowToolBar
    {
        get => GetValue(ShowToolBarProperty);
        set => SetValue(ShowToolBarProperty, value);
    }
    private void test()
    {

    }
    public GOSTextEditor()
    {
        FileProperty.Changed.AddClassHandler<GOSTextEditor>((x, e) => x.ChangeFile());
        ExtensionProperty.Changed.AddClassHandler<GOSTextEditor>((x, e) => x.ChangeExtension());
        IsEditingProperty.Changed.AddClassHandler<GOSTextEditor>((x, e) => x.IsReadOnly = !x.IsEditing);
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        //var wrap = e.NameScope.Find<ToggleButton>("PART_WrapButton");
        //var edit = e.NameScope.Find<ToggleSwitch>("PART_EditSwitch");
        var editor = e.NameScope.Find<TextEditor>("PART_Editor");
        //Document = e.NameScope.Find<TextDocument>("PART_Document");
        //EditorOptions = e.NameScope.Find<TextEditorOptions>("PART_Options");

        Document = editor.Document;
        EditorOptions = editor.Options;
        Document.Changed += Document_Changed;
        EditorOptions.ConvertTabsToSpaces = false;
        EditorOptions.EnableTextDragDrop = true;
        EditorOptions.HighlightCurrentLine = true;
        EditorOptions.HideCursorWhileTyping = true;

        if (!string.IsNullOrWhiteSpace(File))
            ChangeFile();
    }
    private async void Document_Changed(object? sender, DocumentChangeEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(File) && changingFile)
            return;

        if (!isSavingTextSync && !isTextChanged)
            isTextChanged = true;

        var text = GetDocumentText();
        await Task.Delay(2000);
#if DEBUG
        var trash = GetDocumentText();
#endif
        if (text is not null && text != GetDocumentText())
            return;

        if (!isSavingTextSync && isTextChanged)
            FileSave(text, true);
    }
    private async void ChangeFile()
    {
        SaveCurrentDocumentText();
        await SetNewFile();
    }
    private async void ChangeExtension()
    {
        if (string.IsNullOrWhiteSpace(Extension))
        {

        }
        else
        {

        }
    }
}
