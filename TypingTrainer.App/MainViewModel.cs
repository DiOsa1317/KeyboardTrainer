using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TypingTrainer.Core;

namespace TypingTrainer.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IStatsRepository _repository;
    private TypingSession? _session;

    [ObservableProperty] private string _lessonText = "Если поцелуй - это слово, то у меня к тебе серьезный разговор";
    [ObservableProperty] private int _currentIndex;
    [ObservableProperty] private double _wpm;
    [ObservableProperty] private double _accuracy;
    [ObservableProperty] private string _status = "Кликните в окно и начните печатать...";
    
    public MainViewModel(IStatsRepository repository) => _repository = repository;

    public void Initialize()
    {
        _session = new TypingSession(LessonText);
        CurrentIndex = 0;
        UpdateMetrics();
    }

    public void ProcessInput(char typedChar)
    {
        if (_session is null || _session.IsFinished) return;
        _session.TryInput(typedChar);
        _currentIndex = _session.CurrentIndex;
        UpdateMetrics();

        if (_session.IsFinished)
        {
            Status = $"Готово! WPM: {Wpm:F1} | Точность: {Accuracy:F1}%";
            _ = SaveStatsAsync();
        }
    }

    private void UpdateMetrics()
    {
        Wpm = _session!.GetWpm();
        Accuracy = _session.GetAccuracy();
        
        Status = _session!.IsFinished 
            ? "Завершено!" 
            : (_session.IsStarted ? "Печатайте" : "Кликните в окно и начните печатать...");
    }

    private async Task SaveStatsAsync()
    {
        var duration = (int)(_session!.FinishTime!.Value - _session!.StartTime!.Value).TotalSeconds;
        await _repository.SaveSessionAsync(Wpm, Accuracy, duration, _session.WrongChars);
    }

    [RelayCommand]
    private void Restart() => Initialize();
}