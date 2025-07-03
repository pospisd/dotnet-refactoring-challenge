namespace RefactoringChallenge.Abstractions.Data;

public interface ITimeProvider
{
    DateTime Now { get; }
}