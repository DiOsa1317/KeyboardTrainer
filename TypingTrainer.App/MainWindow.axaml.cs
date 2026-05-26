using Avalonia.Controls;
using Avalonia.Input;
using TypingTrainer.App.ViewModels;

namespace TypingTrainer.App;

public partial class MainWindow : Window
{
    private MainViewModel Vm => (MainViewModel)DataContext!;

    public MainWindow() => InitializeComponent();

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text) || char.IsControl(e.Text[0])) return;
        Vm.ProcessInput(e.Text[0]);
        e.Handled = true;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Vm.RestartCommand.Execute(null);
    }
}