using Avalonia;
using System;

namespace TypingTrainer.App;

public class Program
{
    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("DOTNET_MetricsEnabled", "0");
        AppContext.SetSwitch("Npgsql.EnableMetrics", false);
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp()
    =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}