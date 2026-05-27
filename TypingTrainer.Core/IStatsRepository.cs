namespace TypingTrainer.Core;

public interface IStatsRepository
{
    Task SaveSessionAsync(double wpm, double accuracy, int durationSec, int errors);
    Task<IReadOnlyList<SessionStatistics>> GetRecentSessionsAsync(int count = 20);
    Task<IReadOnlyList<Lesson>> GetAllLessonsAsync();
    Task<string?> GetLessonContentAsync(int id);
    
    Task AddLessonAsync(string title, string content, string language);
}