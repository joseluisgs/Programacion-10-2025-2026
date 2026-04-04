using System.Windows;
using System.Windows.Controls;

namespace JuegoMosca.Controls;

public partial class SpinnerControl : UserControl
{
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
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var s = (SpinnerControl)d;
        var v = Math.Clamp((int)e.NewValue, s.Minimum, s.Maximum);
        if (v != (int)e.NewValue) s.Value = v;
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

    private void Up_Click(object sender, RoutedEventArgs e)
    {
        if (Value < Maximum) Value++;
    }

    private void Down_Click(object sender, RoutedEventArgs e)
    {
        if (Value > Minimum) Value--;
    }
}
