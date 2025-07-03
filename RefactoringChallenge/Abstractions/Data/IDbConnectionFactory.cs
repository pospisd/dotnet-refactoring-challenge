using System.Data;

namespace RefactoringChallenge.Abstractions.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}