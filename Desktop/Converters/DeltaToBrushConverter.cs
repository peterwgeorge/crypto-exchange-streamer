using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Desktop.Converters;

public class DeltaToBrushConverter : IValueConverter
{
    public double Deadband { get; set; } = 0.01; // dollars or %

    public Brush PositiveBrush { get; set; } = Brushes.LimeGreen;
    public Brush NegativeBrush { get; set; } = Brushes.IndianRed;
    public Brush NeutralBrush { get; set; } = Brushes.LightGray;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not decimal delta)
            return NeutralBrush;

        if (Math.Abs((double)delta) < Deadband)
            return NeutralBrush;

        return delta > 0 ? PositiveBrush : NegativeBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}