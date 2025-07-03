using RefactoringChallenge.Abstractions.Data;

namespace RefactoringChallenge.Data;

internal class SystemTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}