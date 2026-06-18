using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace StarWars.Controls;

public partial class SpinnerControl : UserControl
{
    private bool _isUpdating;

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(int), typeof(SpinnerControl),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(SpinnerControl),
            new FrameworkPropertyMetadata(1, OnMinChanged));

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(SpinnerControl),
            new FrameworkPropertyMetadata(10, OnMaxChanged));

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public SpinnerControl()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            ValueTextBox.SetBinding(TextBox.TextProperty, new Binding(nameof(Value))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            });
            SyncTextBox();
        };
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var s = (SpinnerControl)d;
        var v = Math.Clamp((int)e.NewValue, s.Minimum, s.Maximum);
        if (v != (int)e.NewValue)
            s.Value = v;
        s.SyncTextBox();
    }

    private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var s = (SpinnerControl)d;
        if (s.Value < s.Minimum) s.Value = s.Minimum;
    }

    private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var s = (SpinnerControl)d;
        if (s.Value > s.Maximum) s.Value = s.Maximum;
    }

    private void SyncTextBox()
    {
        if (_isUpdating || ValueTextBox == null) return;
        _isUpdating = true;
        ValueTextBox.Text = Value.ToString();
        _isUpdating = false;
    }

    private void Up_Click(object sender, RoutedEventArgs e)
    {
        if (Value < Maximum) Value++;
    }

    private void Down_Click(object sender, RoutedEventArgs e)
    {
        if (Value > Minimum) Value--;
    }

    private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
    }

    private void ValueTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var text = (string)e.DataObject.GetData(typeof(string));
            if (!Regex.IsMatch(text, "^[0-9]+$"))
                e.CancelCommand();
        }
        else
        {
            e.CancelCommand();
        }
    }

    private void ValueTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ValueTextBox.Text, out var val))
        {
            Value = Math.Clamp(val, Minimum, Maximum);
        }
        SyncTextBox();
    }
}
