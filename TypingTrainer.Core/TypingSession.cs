namespace TypingTrainer.Core;

public class TypingSession
{
    public string Text { get; }
    public int CurrentIndex { get; private set; }
    public bool IsStarted => StartTime.HasValue;
    public bool IsFinished => CurrentIndex >= Text.Length;
    
    public DateTime? StartTime { get; private set; }
    public DateTime? FinishTime { get; private set; }
    public int CorrectChars {get; private set;}
    public int WrongChars {get; private set;}
    public int TotalAttempts => CorrectChars + WrongChars;
    
    public TypingSession(string text) => Text = text;
    
    public void Start() => StartTime = DateTime.UtcNow;

    public bool TryInput(char typedChar)
    {
        if (IsFinished)
            return false;
        if (!IsStarted)
            Start();
        var isCorrect = typedChar == Text[CurrentIndex];
        if (isCorrect)
            CorrectChars++;
        else
            WrongChars++;
        CurrentIndex++;
        if (IsFinished)
            FinishTime = DateTime.UtcNow;
        return isCorrect;
    }

    public double GetWpm()
    {
        if (!StartTime.HasValue || TotalAttempts  == 0)
            return 0;
        var minutes = (FinishTime ?? DateTime.UtcNow)
            .Subtract(StartTime.Value).TotalMinutes;
        return minutes <=  0 ? 0 : (CorrectChars / 5.0) / minutes;
    }

    public double GetAccuracy()
    {
        return TotalAttempts == 0 ? 100 : (double)CorrectChars / TotalAttempts * 100;
    }
}