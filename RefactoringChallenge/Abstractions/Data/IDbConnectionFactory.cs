using System.Data;

namespace RefactoringChallenge.Abstractions.Data;

/// <summary>
/// Defines a factory for creating instances of <see cref="IDbConnection"/>.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for providing database connection instances
/// configured according to the application's requirements. These connections can be used to interact
/// with the underlying database.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// var connectionFactory = new SqlConnectionFactory(options);
/// using var connection = connectionFactory.CreateConnection();
/// connection.Open();
/// // Perform database operations
/// </code>
/// </example>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and returns a new instance of <see cref="IDbConnection"/>.
    /// </summary>
    /// <returns>
    /// A new <see cref="IDbConnection"/> instance configured according to the implementation.
    /// </returns>
    /// <remarks>
    /// The returned connection is not opened by default. It is the caller's responsibility to open, use, and close the connection.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// var connectionFactory = new SqlConnectionFactory(options);
    /// using var connection = connectionFactory.CreateConnection();
    /// connection.Open();
    /// // Perform database operations
    /// </code>
    /// </example>
    IDbConnection CreateConnection();
}