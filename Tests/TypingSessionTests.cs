using FluentAssertions;
using TypingTrainer.Core;
using Xunit;

namespace TypingTrainer.Tests;

public class TypingSessionTests
{
    [Fact]
    public void Constructor_SetsLessonsText()
    {
        var text = "Hello, world!";
        var session = new TypingSession(text);

        session.Text.Should().Be(text);
        session.CurrentIndex.Should().Be(0);
        session.IsStarted.Should().BeFalse();
        session.IsFinished.Should().BeFalse();
    }

    [Fact]
    public void TryInput_CorrectChar_AdvancesIndex()
    {
        var session = new TypingSession("ABC");
        session.TryInput('A');
        
        session.CurrentIndex.Should().Be(1);
        session.IsStarted.Should().BeTrue();
    }

    [Fact]
    public void TryInput_WrongChar_IncrementsWrongCount()
    {
        var session = new TypingSession("ABC");
        session.TryInput('X');
        session.CurrentIndex.Should().Be(1);
        session.WrongChars.Should().Be(1);
    }

    [Fact]
    public void GetAccuracy_AllCorrect_Returns100()
    {
        var session = new TypingSession("ABC");
        session.TryInput('A');
        session.TryInput('B');
        session.TryInput('C');
        
        var accuracy = session.GetAccuracy();
        accuracy.Should().Be(100.0);
    }

    [Fact]
    public void GetAccuracy_WithErrors_ReturnsCorrectValue()
    {
        var session = new TypingSession("ABCD");
        session.TryInput('A');
        session.TryInput('X');
        session.TryInput('C');
        session.TryInput('D');
        
        var accuracy = session.GetAccuracy();
        accuracy.Should().BeApproximately(75.0, 0.01);
    }
}