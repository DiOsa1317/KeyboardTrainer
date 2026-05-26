namespace TypingTrainer.Core;

public record SessionStatistics(int Id,  double Wpm, 
    double Accuracy, int DurationSec, DateTime CreatedAt);