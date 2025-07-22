using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using static System.Net.Mime.MediaTypeNames;

namespace GOSAvaloniaControls;

public class TextBoxNumber : TemplatedControl
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<TextBoxNumber, double>(nameof(Value), 0, false, BindingMode.TwoWay, enableDataValidation: true);
    public static readonly StyledProperty<int> ValueIntProperty =
        AvaloniaProperty.Register<TextBoxNumber, int>(nameof(ValueInt), 0, false, BindingMode.TwoWay, enableDataValidation: true);
    public static readonly StyledProperty<bool> IsIntegerProperty = AvaloniaProperty.Register<TextBoxNumber, bool>(nameof(IsInteger), false, false, BindingMode.TwoWay);
    public static readonly StyledProperty<string> WatermarkProperty =
        AvaloniaProperty.Register<TextBoxNumber, string>(nameof(Watermark), string.Empty, false, BindingMode.TwoWay, enableDataValidation: true);

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
    public TextBoxNumber()
    {
        ValueProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeValueDouble());
        ValueIntProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeValueInt());
        IsIntegerProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeIsInteger());
        WatermarkProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) =>
        {
            if (x._textBox is not null)
                x._textBox.Watermark = x.Watermark;
        });
        //Monitora a digitação no TextBox ao pressionar uma tecla
        this.TextInput += TextBoxNumber_TextInput;
        //Monitora o texto do TextBox
    }
    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        base.UpdateDataValidation(property, state, error);
        //Tem que colocar este override para funcionar a validação de dados
        if (property == ValueProperty || property == ValueIntProperty)
        {
            DataValidationErrors.SetError(this, error);
            if (_textBox is not null)
                DataValidationErrors.SetError(_textBox, error);

        }
    }
    TextBox? _textBox = null;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _textBox.Watermark = Watermark;
        _textBox.TextChanged += _textBox_TextChanged;
        if (IsInteger)
        {
            ChangeValueInt();
        }
        else
        {
            ChangeValueDouble();
        }
        //if (string.IsNullOrEmpty(_textBox?.Text) && string.IsNullOrWhiteSpace(Watermark))
        //{
        //    _textBox!.Text = 0.ToString();
        //}
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
                return;
            }
        }
        else
        {
            if (double.TryParse(textBox.Text, out double valueDoube))
            {
                if (Value != valueDoube)
                    Value = valueDoube;
                return;
            }
        }
        textBox.Text = Value.ToString();
    }

    private void TextBoxNumber_TextInput(object? sender, Avalonia.Input.TextInputEventArgs e)
    {
        //Se o texto digitado não for um número, ponto, vírgula, sinal de menos ou sinal de mais
        //if (e.Text is null ||
        //        (IsInteger && e.Text.Any(x => !char.IsDigit(x)) && e.Text != "-" && e.Text != "+") ||
        //        (!IsInteger && e.Text.Any(x => !char.IsDigit(x)) && e.Text != "." && e.Text != "," && e.Text != "-" && e.Text != "+"))
        //{
        //    e.Handled = true;
        //}
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

            if (double.TryParse(_textBox.Text, out double valueDoube))
            {
                if (Value == valueDoube)
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

