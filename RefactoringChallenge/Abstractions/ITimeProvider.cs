namespace RefactoringChallenge.Abstractions;

public interface ITimeProvider
{
    DateTime Now { get; }
}