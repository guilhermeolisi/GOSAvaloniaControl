using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace GOSAvaloniaControls;

public class TextBoxNumber : TextBox
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<TextBoxNumber, double>(nameof(Value), 0, false, BindingMode.TwoWay, enableDataValidation: true);
    public static readonly StyledProperty<int> ValueIntProperty =
        AvaloniaProperty.Register<TextBoxNumber, int>(nameof(ValueInt), 0, false, BindingMode.TwoWay, enableDataValidation: true);
    public static readonly StyledProperty<bool> IsIntegerProperty = AvaloniaProperty.Register<TextBoxNumber, bool>(nameof(IsInteger), false, false, BindingMode.TwoWay);

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
    public TextBoxNumber()
    {
        ValueProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeValue());
        ValueIntProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeValueInt());
        IsIntegerProperty.Changed.AddClassHandler<TextBoxNumber>((x, e) => x.ChangeIsInteger());
        //Monitora a digitação no TextBox ao pressionar uma tecla
        this.TextInput += TextBoxNumber_TextInput;
        //Monitora o texto do TextBox
        this.PropertyChanged += TextBoxNumber_PropertyChanged;

    }
    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        //Tem que colocar este override para funcionar a validação de dados
        if (property == ValueProperty || property == ValueIntProperty)
        {
            DataValidationErrors.SetError(this, error);
        }
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (string.IsNullOrEmpty(Text))
        {
            Text = 0.ToString();
        }
    }
    private void TextBoxNumber_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        //Se o texto for um numero atualiza o valor
        if (e.Property == TextProperty)
        {
            if (string.IsNullOrEmpty(Text) || Text == "-" || Text == "+")
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
                if (int.TryParse(Text, out int valueInt))
                {
                    if (ValueInt != valueInt)
                        ValueInt = valueInt;
                    return;
                }
            }
            else
            {
                if (double.TryParse(Text, out double valueDoube))
                {
                    if (Value != valueDoube)
                        Value = valueDoube;
                    return;
                }
            }
            Text = Value.ToString();
        }
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
    private void ChangeValue()
    {
        string temp = Value.ToString();
        if (Text != temp)
        {
            if (string.IsNullOrWhiteSpace(Text) && Value == 0 && isSetValueToZeroFromText)
            {
                return;
            }

            if (double.TryParse(Text, out double valueDoube))
            {
                if (Value == valueDoube)
                    return;
            }
            Text = temp;
        }
    }
    private void ChangeValueInt()
    {
        string temp = ValueInt.ToString();
        if (Text != temp)
        {
            if (string.IsNullOrWhiteSpace(Text) && ValueInt == 0 && isSetValueToZeroFromText)
            {
                return;
            }

            if (int.TryParse(Text, out int valueInt))
            {
                if (ValueInt == valueInt)
                    return;
            }
            Text = temp;
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

