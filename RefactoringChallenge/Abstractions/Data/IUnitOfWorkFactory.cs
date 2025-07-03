namespace RefactoringChallenge.Abstractions.Data;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}