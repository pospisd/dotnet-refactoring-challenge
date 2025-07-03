using RefactoringChallenge.Abstractions.Data;

namespace RefactoringChallenge.Data;

internal sealed class SqlUnitOfWorkFactory(IDbConnectionFactory connectionFactory) : IUnitOfWorkFactory
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    public IUnitOfWork Create()
    {
        return new SqlUnitOfWork(_connectionFactory);
    }
}