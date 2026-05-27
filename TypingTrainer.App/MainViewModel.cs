using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Avalonia.Media;
using TypingTrainer.Core;

namespace TypingTrainer.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IStatsRepository _repository;
    private TypingSession? _session;
    
    public ObservableCollection<Lesson> Lessons { get;} = new();
    
    [ObservableProperty]
    private Lesson? _selectedLesson;

    [ObservableProperty]
    private string _lessonText = "Если поцелуй - это слово, то у меня к тебе серьезный разговор";
    [ObservableProperty] 
    private int _currentIndex;
    [ObservableProperty]
    private double _wpm;
    [ObservableProperty]
    private double _accuracy;
    [ObservableProperty]
    private string _status = "Кликните в окно и начните печатать...";
    
    public ObservableCollection<CharItemViewModel> CharacterItems { get; } = new();
    private readonly List<bool?> _inputResults = new();
    
    public MainViewModel(IStatsRepository repository) => _repository = repository;

    public async Task InitializeAsync()
    {
        try
        {
            // Пробуем загрузить уроки из репозитория
            var dbLessons = await _repository.GetAllLessonsAsync();
        
            Lessons.Clear();
            foreach (var lesson in dbLessons)
            {
                Lessons.Add(lesson);
            }

            if (Lessons.Count > 0 && SelectedLesson == null)
            {
                SelectedLesson = Lessons[0];
            }

            StartNewSession(SelectedLesson);
        }
        catch (System.Exception ex)
        {
            // Если что-то пойдёт не так (нет таблицы, неверный пароль, нет связи),
            // текст ошибки запишется в статус-бар внизу окна!
            Status = $"⚠️ Ошибка подключения к БД: {ex.Message}";
        }
    }

    private void StartNewSession(Lesson? lesson)
    {
        if (lesson is null)
            return;
        LessonText = lesson.Content;
        
        _lessonText = _lessonText.Replace('—', '-')
            .Replace('–', '-')
            .Replace('−', '-');
        _session = new TypingSession(LessonText);
        CurrentIndex = 0;
        
        _inputResults.Clear();
        for (var i = 0; i < _lessonText.Length; i++)
        {
            _inputResults.Add(null);
        }

        BuildCharacterItems();
        UpdateMetrics();
    }

    partial void OnSelectedLessonChanged(Lesson? value)
    {
        if (value is not null)
        {
            StartNewSession(value);
        }
    }

    public void ProcessInput(char typedChar)
    {
        if (_session is null || _session.IsFinished) return;

        var indexBeforeInput = _session.CurrentIndex;
        var targetChar = _lessonText[indexBeforeInput];
        
        if(IsWrongLayout(typedChar, targetChar))
            return;
        
        var isCorrect = typedChar == targetChar;
        
        _inputResults[indexBeforeInput] = isCorrect;
        
        _session.TryInput(typedChar);
        CurrentIndex = _session.CurrentIndex;
        
        BuildCharacterItems();
        UpdateMetrics();
        
        if (_session.IsFinished)
        {
            Status = $"Готово! WPM: {Wpm:F1} | Точность: {Accuracy:F1}%";
            _ = SaveStatsAsync();
        }
    }

    private bool IsWrongLayout(char typedChar, char targetChar)
    {
        if(!char.IsLetter(typedChar) || !char.IsLetter(targetChar))
            return false;
        var isInputEnglish = (typedChar >= 'a' && typedChar <= 'z') || (typedChar >= 'A' && typedChar <= 'Z');
        var isTargetEnglish = (targetChar >= 'a' && targetChar <= 'z') || (targetChar >= 'A' && targetChar <= 'Z');
        var isInputRussian = (typedChar >= 'а' && typedChar <= 'я') || (typedChar >= 'А' && typedChar <= 'Я')
                                                                    || typedChar == 'ё' || typedChar == 'Ё';
        var isTargetRussian = (targetChar >= 'а' && targetChar <= 'я') || (targetChar >= 'А' && targetChar <= 'Я')
                                                                       || targetChar == 'ё' || targetChar == 'Ё';

        if (isTargetRussian && isInputEnglish)
        {
            Status = "Смените раскладку на русскую";
            return true;
        }

        if (isInputRussian && isTargetEnglish)
        {
            Status = "Смените раскладку на английскую";
            return true;
        }
        return false;
    }

    private void BuildCharacterItems()
    {
        CharacterItems.Clear();
        for (var i = 0; i < _lessonText.Length; i++)
        {
            var originalChar = _lessonText[i];
            var item = new CharItemViewModel();

            if (originalChar == ' ')
            {
                item.CharValue = " ";
                item.Width = 12;
            }
            else
            {
                item.CharValue = originalChar.ToString();
            }

            if (i == CurrentIndex)
            {
                item.Color = Brushes.Orange;
                item.Decoration = TextDecorations.Underline;
                if (originalChar == ' ')
                    item.CharValue = "␣";
            }
            else if (_inputResults[i] == true)
            {
                item.Color = Brushes.LightGreen;
            }
            else if (_inputResults[i] == false)
            {
                item.Color = Brushes.Crimson;
                if (originalChar == ' ')
                    item.Color = Brushes.DarkRed;
            }
            else
            {
                item.Color = Brushes.Gray;
            }
            CharacterItems.Add(item);
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
    private void Restart()
    {
        if (SelectedLesson is not null)
        {
            StartNewSession(SelectedLesson);
        }
    }
}