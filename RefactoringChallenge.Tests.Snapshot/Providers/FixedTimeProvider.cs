using RefactoringChallenge.Abstractions;

namespace RefactoringChallenge.Tests.Snapshot.Providers;

public class FixedTimeProvider(DateTime fixedNow) : ITimeProvider
{
    public DateTime Now { get; } = fixedNow;
}