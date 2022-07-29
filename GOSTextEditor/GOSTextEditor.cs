using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using BaseLibrary;
using Nimloth.TextMate.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GOSAvaloniaControls;

public partial class GOSTextEditor : TemplatedControl
{
    public static readonly StyledProperty<string?> FilePathProperty = AvaloniaProperty.Register<GOSTextEditor, string?>(nameof(FilePath));
    //public static readonly DirectProperty<TextEditorTemplated, string?> FileProperty =        AvaloniaProperty.RegisterDirect<TextEditorTemplated, string?>(nameof(File), o => o.File, (o, v) => o.File = v);
    public static readonly StyledProperty<string?> ExtensionProperty = AvaloniaProperty.Register<GOSTextEditor, string?>(nameof(Extension));
    public static readonly StyledProperty<bool> IsEditingProperty = AvaloniaProperty.Register<GOSTextEditor, bool>(nameof(IsEditing), true, false, BindingMode.TwoWay);
    public static readonly DirectProperty<GOSTextEditor, bool> IsReadOnlyProperty = AvaloniaProperty.RegisterDirect<GOSTextEditor, bool>(nameof(IsReadOnly), o => o.IsReadOnly);
    public static readonly StyledProperty<bool> IsWrapProperty = AvaloniaProperty.Register<GOSTextEditor, bool>(nameof(IsWrap), true, false, BindingMode.TwoWay);
    public static readonly StyledProperty<bool> ShowToolBarProperty = AvaloniaProperty.Register<GOSTextEditor, bool>(nameof(ShowToolBar), true, false, BindingMode.OneWay);
    public static readonly StyledProperty<bool> ThemeProperty = AvaloniaProperty.Register<GOSTextEditor, bool>(nameof(Theme), true, false, BindingMode.OneWay);

    public string? FilePath
    {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }
    //private string? _file;
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
    /// <summary>
    /// True = Dark; False = Ligth
    /// </summary>
    public bool Theme
    {
        get => GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }
    public GOSTextEditor()
    {
        _registryOptions = new RegistryOptions(ThemeName.Dark);
        _sindarinLanguage = _registryOptions.GetLanguageByExtension(".sin");

        FilePathProperty.Changed.AddClassHandler<GOSTextEditor>((x, e) => x.ChangeFile());
        ExtensionProperty.Changed.AddClassHandler<GOSTextEditor>((x, e) => x.ChangeExtension());
        ThemeProperty.Changed.AddClassHandler<GOSTextEditor>((x, e) => x.ChangeTheme());
        IsEditingProperty.Changed.AddClassHandler<GOSTextEditor>((x, e) => x.IsReadOnly = !x.IsEditing);
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var wrap = e.NameScope.Find<ToggleButton>("PART_WrapCheck");
        var edit = e.NameScope.Find<ToggleButton>("PART_EditCheck");

        //https://github.com/AvaloniaUI/Avalonia/issues/4616
        //http://reference.avaloniaui.net/api/Avalonia.Controls/ResourceDictionary/50FEA02D
        //if (Application.Current.TryFindResource(""))
        //{
        //    wrap[ForegroundProperty] = new DynamicResourceExtension("MyResource");
        //}

        _editor = e.NameScope.Find<TextEditor>("PART_Editor");

        _textMateInstallation = _editor.InstallTextMate(_registryOptions);

        Document = _editor.Document;
        EditorOptions = _editor.Options;
        Document.Changed += Document_Changed;
        EditorOptions.ConvertTabsToSpaces = false;
        EditorOptions.EnableTextDragDrop = true;
        EditorOptions.HighlightCurrentLine = true;
        EditorOptions.HideCursorWhileTyping = true;

        if (!string.IsNullOrWhiteSpace(FilePath))
            ChangeFile();
        if (isEditNull)
        {
            ChangeExtension();
            ChangeTheme();
        }
    }
    private async void Document_Changed(object? sender, DocumentChangeEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(FilePath) && changingFile)
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

    readonly Language _sindarinLanguage;
    readonly RegistryOptions _registryOptions;
    private TextEditor _editor;
    private AvaloniaEdit.TextMate.TextMate.Installation _textMateInstallation;
    private async void ChangeExtension()
    {
        if (_textMateInstallation is null)
        {
            if (!isEditNull)
                isEditNull = true;
            return;

        }
        isEditNull = false;
        Task? task = null;
        if (string.IsNullOrWhiteSpace(Extension))
        {
            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                //task = new(() => _textMateInstallation.SetGrammar(_registryOptions.GetScopeByExtension(Path.GetExtension(File))));
#if DEBUG
                var trash = _registryOptions.GetScopeByExtension(Path.GetExtension(FilePath));
#endif
                _textMateInstallation.SetGrammar(_registryOptions.GetScopeByExtension(Path.GetExtension(FilePath)));
            }
        }
        else
        {
            if (Extension == ".SIN" || Extension == ".DAT")
            {
                //task = new(() => _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_sindarinLanguage.Id)));
                _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_sindarinLanguage.Id));
            }
            else
            {
                //task = new(() => _textMateInstallation.SetGrammar(_registryOptions.GetScopeByExtension(Extension)));
                _textMateInstallation.SetGrammar(_registryOptions.GetScopeByExtension(Extension));
            }
        }
        if (task is not null)
        {
            task.Start();
            await task;
        }
    }
    bool isEditNull = false;
    private async void ChangeTheme()
    {
        if (_textMateInstallation is null)
        {
            if (!isEditNull)
                isEditNull = true;
            return;

        }
        isEditNull = false;
        Task? task = null;
        if (Theme)
        {
            //task = new(() => _textMateInstallation.SetTheme(_registryOptions.LoadTheme(ThemeName.DarkPlus)));
            _textMateInstallation.SetTheme(_registryOptions.LoadTheme(ThemeName.DarkPlus));
        }
        else
        {
            //task = new(() => _textMateInstallation.SetTheme(_registryOptions.LoadTheme(ThemeName.LightPlus)));
            _textMateInstallation.SetTheme(_registryOptions.LoadTheme(ThemeName.LightPlus));
        }
        if (task is not null)
        {
            task.Start();
            await task;
        }
    }
}
