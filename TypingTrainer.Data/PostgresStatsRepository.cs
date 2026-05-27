using Dapper;
using Npgsql;
using TypingTrainer.Core;

namespace TypingTrainer.Data;

public class PostgresStatsRepository(string connectionString) : IStatsRepository
{
    private readonly string _connectionString = connectionString;

    public async Task SaveSessionAsync(double wpm, double accuracy, int durationSec, int errors)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(@"
        INSERT INTO typing_sessions (wpm, accuracy, duration_sec, errors_count, created_at)
        VALUES (@Wpm, @Accuracy, @DurationSEc, @Errors, NOW())",
            new {Wpm = wpm, DurationSec = durationSec, Accuracy = accuracy, Errors = errors});
    }

    public async Task<IReadOnlyList<SessionStatistics>> GetRecentSessionsAsync(int count = 20)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return ( await connection.QueryAsync<SessionStatistics>(
                @"SELECT id, wpm, accuracy, duration_sec, created_at
                    FROM typing_sessions
                    ORDER BY created_at DESC 
                    LIMIT @Count",
                new {Count = count}))
            .ToList();
    }

    public async Task<IReadOnlyList<Lesson>> GetAllLessonsAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = "SELECT id, title, language FROM lessons ORDER BY id";
        var lessons = await connection.QueryAsync<Lesson>(sql);
        return lessons.ToList();
    }

    public async Task<string> GetLessonContentAsync(int id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var sql = "SELECT content FROM lessons WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<string>(sql, new { Id = id }) ?? string.Empty;
    }

    public async Task AddLessonAsync(string title, string content, string language)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
        INSERT INTO lessons  (title, content, language)
        VALUES (@Title, @Content, @Language)";
        
        await connection
            .ExecuteAsync(sql, 
                new {Title = title, Content = content, Language = language});
    }
}