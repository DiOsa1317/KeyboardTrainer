using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TypingTrainer.App.ViewModels;
using TypingTrainer.Core;
using TypingTrainer.Data;

namespace TypingTrainer.App;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(config);
                services.AddSingleton<IStatsRepository>(sp =>
                    new PostgresStatsRepository(config.GetConnectionString("Postgres")!));
                services.AddSingleton<MainViewModel>();
            })
            .Build();

        var vm = host.Services.GetRequiredService<MainViewModel>();
        vm.Initialize();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow {DataContext = vm};
            desktop.MainWindow.Show();
        }
        base.OnFrameworkInitializationCompleted();
    }
}