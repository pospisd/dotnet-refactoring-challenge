using System.Data;
using RefactoringChallenge.Abstractions.Data;

namespace RefactoringChallenge.Data;

/// <summary>
/// Provides an implementation of the <see cref="IUnitOfWork"/> interface using SQL-based database connections.
/// </summary>
/// <remarks>
/// This class manages a database connection and transaction lifecycle, ensuring that operations can be grouped 
/// into a single atomic transaction. It utilizes an <see cref="IDbConnectionFactory"/> to create database connections.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// using (var unitOfWork = new SqlUnitOfWork(connectionFactory))
/// {
///     unitOfWork.Begin();
///     try
///     {
///         // Perform database operations here
///         unitOfWork.Commit();
///     }
///     catch
///     {
///         unitOfWork.Rollback();
///         throw;
///     }
/// }
/// </code>
/// </example>
internal sealed class SqlUnitOfWork(IDbConnectionFactory connectionFactory) : IUnitOfWork
{
    /// <summary>
    /// Gets the database connection associated with the current unit of work.
    /// </summary>
    /// <remarks>
    /// The connection is created using the provided <see cref="IDbConnectionFactory"/> and is managed 
    /// by the <see cref="SqlUnitOfWork"/> class. It is used to execute database commands and manage 
    /// transactions within the unit of work.
    /// </remarks>
    /// <value>
    /// An <see cref="IDbConnection"/> instance representing the database connection.
    /// </value>
    /// <example>
    /// Example usage:
    /// <code>
    /// using (var unitOfWork = new SqlUnitOfWork(connectionFactory))
    /// {
    ///     var connection = unitOfWork.Connection;
    ///     // Use the connection to execute database commands
    /// }
    /// </code>
    /// </example>
    public IDbConnection Connection { get; } = connectionFactory.CreateConnection();

    /// <summary>
    /// Gets the current database transaction associated with this unit of work.
    /// </summary>
    /// <remarks>
    /// This property holds the active <see cref="IDbTransaction"/> instance, if any, 
    /// that is used to group multiple database operations into a single atomic transaction.
    /// It is initialized when <see cref="Begin"/> is called and disposed after 
    /// <see cref="Commit"/> or <see cref="Rollback"/> is executed.
    /// </remarks>
    /// <value>
    /// The current <see cref="IDbTransaction"/> instance, or <c>null</c> if no transaction is active.
    /// </value>
    public IDbTransaction? Transaction { get; private set; }

    /// <summary>
    /// Begins a new database transaction for the current unit of work.
    /// </summary>
    /// <remarks>
    /// This method ensures that the database connection is open and starts a new transaction.
    /// If a transaction is already active, it will be replaced by the new transaction.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the database connection cannot be opened or is in an invalid state.
    /// </exception>
    /// <example>
    /// Example usage:
    /// <code>
    /// using (var unitOfWork = new SqlUnitOfWork(connectionFactory))
    /// {
    ///     unitOfWork.Begin();
    ///     try
    ///     {
    ///         // Perform database operations here
    ///         unitOfWork.Commit();
    ///     }
    ///     catch
    ///     {
    ///         unitOfWork.Rollback();
    ///         throw;
    ///     }
    /// }
    /// </code>
    /// </example>
    public void Begin()
    {
        if (Connection.State != System.Data.ConnectionState.Open)
            Connection.Open();

        Transaction = Connection.BeginTransaction();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Commit()
    {
        Transaction?.Commit();
        Transaction?.Dispose();
        Transaction = null;
    }

    /// <summary>
    /// Rolls back the current database transaction, if one is active.
    /// </summary>
    /// <remarks>
    /// This method reverts all changes made during the current transaction and disposes of the 
    /// <see cref="IDbTransaction"/> instance. After calling this method, the <see cref="Transaction"/> 
    /// property will be set to <c>null</c>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the transaction is not active or has already been committed or rolled back.
    /// </exception>
    /// <example>
    /// Example usage:
    /// <code>
    /// try
    /// {
    ///     unitOfWork.Begin();
    ///     // Perform database operations
    ///     unitOfWork.Commit();
    /// }
    /// catch
    /// {
    ///     unitOfWork.Rollback();
    ///     throw;
    /// }
    /// </code>
    /// </example>
    public void Rollback()
    {
        Transaction?.Rollback();
        Transaction?.Dispose();
        Transaction = null;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="SqlUnitOfWork"/> instance.
    /// </summary>
    /// <remarks>
    /// This method disposes of the current database transaction, if any, and closes the database connection.
    /// It ensures that all resources are properly cleaned up to avoid potential memory leaks or resource locks.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// using (var unitOfWork = new SqlUnitOfWork(connectionFactory))
    /// {
    ///     unitOfWork.Begin();
    ///     try
    ///     {
    ///         // Perform database operations here
    ///         unitOfWork.Commit();
    ///     }
    ///     catch
    ///     {
    ///         unitOfWork.Rollback();
    ///         throw;
    ///     }
    /// }
    /// // At this point, Dispose is automatically called to release resources.
    /// </code>
    /// </example>
    public void Dispose()
    {
        Transaction?.Dispose();
        Connection.Dispose();
    }
}