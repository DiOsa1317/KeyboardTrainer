using FluentAssertions;
using Moq;
using TypingTrainer.App.ViewModels;
using TypingTrainer.Core;

namespace TypingTrainer.Tests;

public class MainViewModelTests
{
    [Fact]
    public async Task Initialize_SetsSessionAndMetrics()
    {
        var mockRepo = new Mock<IStatsRepository>();
        var vm = new MainViewModel(mockRepo.Object);

        await vm.InitializeAsync();
        
        vm.LessonText.Should().NotBeNullOrEmpty();
        vm.CurrentIndex.Should().Be(0);
       // vm.Status.Should().Contain("Начните печатать");
    }

    // [Fact]
    // public async Task ProcessInput_Finished_SavesStats()
    // {
    //     var mockRepo = new Mock<IStatsRepository>();
    //     var vm = new MainViewModel(mockRepo.Object);
    //     await vm.InitializeAsync();
    //
    //     foreach (var c in vm.LessonText)
    //         vm.ProcessInput(c);
    //     
    //     vm.Status.Should().Contain("Готово");
    //     
    //     mockRepo.Verify(r => r.SaveSessionAsync(
    //         It.IsAny<double>()));
    // }
}