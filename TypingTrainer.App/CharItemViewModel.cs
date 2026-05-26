using Avalonia.Media;

namespace TypingTrainer.App.ViewModels;

public class CharItemViewModel
{
    public string CharValue { get; set; } = string.Empty;
    public IBrush Color { get; set; } = Brushes.Gray;
    public TextDecorationCollection? Decoration {get; set; }
    public double? Width { get; set; }
}