using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace GOSAvaloniaControls;

public class TextBoxNumber : TemplatedControl
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<TextBoxNumber, double>(nameof(Value), 0, false, BindingMode.TwoWay, enableDataValidation: true);
    public static readonly StyledProperty<int> ValueIntProperty =
        AvaloniaProperty.Register<TextBoxNumber, int>(nameof(ValueInt), 0, false, BindingMode.TwoWay, enableDataValidation: true);
    public static readonly StyledProperty<bool> IsIntegerProperty =
        AvaloniaProperty.Register<TextBoxNumber, bool>(nameof(IsInteger), false, false, BindingMode.TwoWay);
    public static readonly StyledProperty<string> WatermarkProperty =
        AvaloniaProperty.Register<TextBoxNumber, string>(nameof(Watermark), string.Empty, false, BindingMode.TwoWay, enableDataValidation: true);
    public static readonly StyledProperty<bool> UseFloatingWatermarkProperty =
        AvaloniaProperty.Register<TextBoxNumber, bool>(nameof(UseFloatingWatermark), true, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MinValueProperty =
        AvaloniaProperty.Register<TextBoxNumber, double>(nameof(MinValue), double.MinValue, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxValueProperty =
        AvaloniaProperty.Register<TextBoxNumber, double>(nameof(MaxValue), double.MaxValue, false, BindingMode.TwoWay);
    public static readonly StyledProperty<int> ValidationDelayProperty =
        AvaloniaProperty.Register<TextBoxNumber, int>(nameof(ValidationDelay), 3000, false, BindingMode.TwoWay);

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    public int ValueInt
    {
        get => GetValue(ValueIntProperty);
        set => SetValue(ValueIntProperty, value);
    }
    public bool IsInteger
    {
        get => GetValue(IsIntegerProperty);
        set => SetValue(IsIntegerProperty, value);
    }
    public string Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }
    public bool UseFloatingWatermark
    {
        get => GetValue(UseFloatingWatermarkProperty);
        set => SetValue(UseFloatingWatermarkProperty, value);
    }
    public double MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }
    public double MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }
    public int ValidationDelay
    {
        get => GetValue(ValidationDelayProperty);
        set => SetValue(ValidationDelayProperty, value);
    }

    static TextBoxNumber()
    {
        ValueProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeValueDouble());
        ValueIntProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeValueInt());
        IsIntegerProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeIsInteger());
        WatermarkProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) =>
        {
            if (x._textBox is not null)
                x._textBox.Watermark = x.Watermark;
        });
        UseFloatingWatermarkProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) =>
        {
            if (x._textBox is not null)
            {
                x._textBox.UseFloatingWatermark = x.UseFloatingWatermark;
            }
        });
    }

    public TextBoxNumber()
    {
    }

    protected override void OnDetachedFromLogicalTree(Avalonia.LogicalTree.LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _validationCts?.Cancel();
        _validationCts?.Dispose();
        _validationCts = null;
    }

    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        base.UpdateDataValidation(property, state, error);
        if (property == ValueProperty || property == ValueIntProperty)
        {
            DataValidationErrors.SetError(this, error);
            if (_textBox is not null)
                DataValidationErrors.SetError(_textBox, error);
        }
    }

    TextBox? _textBox = null;
    private CancellationTokenSource? _validationCts;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _textBox!.Watermark = Watermark;
        _textBox.TextChanged += _textBox_TextChanged;
        if (IsInteger)
        {
            ChangeValueInt();
        }
        else
        {
            ChangeValueDouble();
        }
        _textBox.UseFloatingWatermark = UseFloatingWatermark;
    }

    private void _textBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.Source is not TextBox textBox)
            return;
        if (_textBox is null)
            return;

        if (string.IsNullOrEmpty(textBox.Text) || textBox.Text == "-" || textBox.Text == "+")
        {
            isSetValueToZeroFromText = true;
            if (IsInteger)
            {
                if (ValueInt != 0)
                    ValueInt = 0;
            }
            else
            {
                if (Value != 0)
                    Value = 0;
            }
            isSetValueToZeroFromText = false;
            return;
        }
        if (IsInteger)
        {
            if (int.TryParse(textBox.Text, out int valueInt))
            {
                if (ValueInt != valueInt)
                    ValueInt = valueInt;
                ScheduleValidation();
                return;
            }
        }
        else
        {
            if (double.TryParse(textBox.Text, out double valueDouble))
            {
                if (Value != valueDouble)
                    Value = valueDouble;
                ScheduleValidation();
                return;
            }
        }
        textBox.Text = Value.ToString();
    }

    private void ScheduleValidation()
    {
        _validationCts?.Cancel();
        _validationCts?.Dispose();
        _validationCts = new CancellationTokenSource();
        var token = _validationCts.Token;
        var delay = ValidationDelay;
        Task.Delay(delay, token).ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully)
                Dispatcher.UIThread.Post(ValidateAndClamp);
        }, TaskScheduler.Default);
    }

    private void ValidateAndClamp()
    {
        if (IsInteger)
        {
            int min = (int)Math.Clamp(MinValue, int.MinValue, int.MaxValue);
            int max = (int)Math.Clamp(MaxValue, int.MinValue, int.MaxValue);
            if (ValueInt < min) ValueInt = min;
            else if (ValueInt > max) ValueInt = max;
        }
        else
        {
            if (Value < MinValue) Value = MinValue;
            else if (Value > MaxValue) Value = MaxValue;
        }
    }

    bool isSetValueToZeroFromText = false;
    private void ChangeValueDouble()
    {
        if (_textBox is null)
            return;
        string temp = Value.ToString();
        if (_textBox.Text != temp)
        {
            if (string.IsNullOrWhiteSpace(_textBox.Text) && Value == 0 && isSetValueToZeroFromText)
            {
                return;
            }

            if (double.TryParse(_textBox.Text, out double valueDouble))
            {
                if (Value == valueDouble)
                    return;
            }
            _textBox.Text = temp;
        }
    }

    private void ChangeValueInt()
    {
        if (_textBox is null)
            return;

        string temp = ValueInt.ToString();
        if (_textBox.Text != temp)
        {
            if (string.IsNullOrWhiteSpace(_textBox.Text) && ValueInt == 0 && isSetValueToZeroFromText)
            {
                return;
            }

            if (int.TryParse(_textBox.Text, out int valueInt))
            {
                if (ValueInt == valueInt)
                    return;
            }
            _textBox.Text = temp;
        }
    }

    private void ChangeIsInteger()
    {
        if (IsInteger)
        {
            ValueInt = (int)Value;
            Value = 0;
        }
        else
        {
            Value = ValueInt;
            ValueInt = 0;
        }
    }
}
