using RefactoringChallenge.Abstractions.Data;

namespace RefactoringChallenge.Data;

/// <summary>
/// Provides a factory for creating instances of <see cref="IUnitOfWork"/> 
/// that are backed by SQL database connections.
/// </summary>
/// <remarks>
/// This class is responsible for creating unit-of-work instances that manage 
/// database transactions and operations using a SQL database connection. 
/// It relies on an implementation of <see cref="IDbConnectionFactory"/> to 
/// provide the necessary database connections.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// var connectionFactory = new SqlConnectionFactory(options);
/// var unitOfWorkFactory = new SqlUnitOfWorkFactory(connectionFactory);
/// using var unitOfWork = unitOfWorkFactory.Create();
/// // Perform operations within the unit of work
/// </code>
/// </example>
internal sealed class SqlUnitOfWorkFactory(IDbConnectionFactory connectionFactory) : IUnitOfWorkFactory
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    /// <summary>
    /// Creates a new instance of <see cref="IUnitOfWork"/> for managing database transactions.
    /// </summary>
    /// <returns>A new <see cref="IUnitOfWork"/> instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the underlying database connection cannot be established.
    /// </exception>
    public IUnitOfWork Create()
    {
        return new SqlUnitOfWork(_connectionFactory);
    }
}