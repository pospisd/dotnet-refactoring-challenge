using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Configuration;

namespace RefactoringChallenge.Data;

/// <summary>
/// Provides a factory for creating SQL database connections.
/// </summary>
/// <remarks>
/// This class is responsible for creating instances of <see cref="IDbConnection"/> using the connection string
/// provided in the application's configuration. It implements the <see cref="IDbConnectionFactory"/> interface
/// to ensure compatibility with other components that rely on database connection factories.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// var options = Options.Create(new DbSettings { ConnectionString = "YourConnectionString" });
/// var connectionFactory = new SqlConnectionFactory(options);
/// using var connection = connectionFactory.CreateConnection();
/// connection.Open();
/// // Perform database operations
/// </code>
/// </example>
internal sealed class SqlConnectionFactory(IOptions<DbSettings> options) : IDbConnectionFactory
{
    private readonly string _connectionString = options.Value.ConnectionString
                                                ?? throw new ArgumentNullException(nameof(options.Value.ConnectionString));

    /// <summary>
    /// Creates and returns a new instance of <see cref="IDbConnection"/> configured with the connection string.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IDbConnection"/> that can be used to interact with the database.
    /// </returns>
    /// <remarks>
    /// The returned connection is not open. It is the caller's responsibility to open and close the connection.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the connection string is null or not properly configured.
    /// </exception>
    /// <example>
    /// Example usage:
    /// <code>
    /// var connectionFactory = new SqlConnectionFactory(Options.Create(new DbSettings { ConnectionString = "YourConnectionString" }));
    /// using var connection = connectionFactory.CreateConnection();
    /// connection.Open();
    /// // Perform database operations
    /// </code>
    /// </example>
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}