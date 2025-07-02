namespace RefactoringChallenge;

internal class SystemTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}