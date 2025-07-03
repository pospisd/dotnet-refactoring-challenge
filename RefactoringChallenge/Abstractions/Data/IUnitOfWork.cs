using System.Data;
using Microsoft.Data.SqlClient;

namespace RefactoringChallenge.Abstractions.Data;

/// <summary>
/// Represents a unit of work that encapsulates a set of operations to be executed as a single transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the SQL database connection associated with the unit of work.
    /// </summary>
    /// <remarks>
    /// This connection is used to execute database operations within the context of the unit of work.
    /// </remarks>
    IDbConnection Connection { get; }
    
    /// <summary>
    /// Gets the current SQL transaction associated with the unit of work, or <c>null</c> if no transaction is active.
    /// </summary>
    IDbTransaction? Transaction { get; }
    
    /// <summary>
    /// Begins a new transaction within the current unit of work.
    /// </summary>
    /// <remarks>
    /// This method initializes a transaction that can be used to group multiple operations 
    /// into a single atomic operation. Ensure to call <see cref="Commit"/> to finalize the transaction 
    /// or <see cref="Rollback"/> to revert changes if an error occurs.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a transaction is already active when attempting to begin a new one.
    /// </exception>
    void Begin();
    
    /// <summary>
    /// Commits the current transaction, persisting all changes made within the unit of work.
    /// </summary>
    /// <remarks>
    /// This method finalizes the transaction, ensuring that all operations performed within the unit of work 
    /// are saved to the database. If no transaction is active, this method has no effect.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there is no active transaction to commit.
    /// </exception>
    void Commit();
    
    /// <summary>
    /// Rolls back the current transaction, reverting all changes made within the unit of work.
    /// </summary>
    /// <remarks>
    /// This method should be called when an error occurs or when the operations within the transaction 
    /// need to be discarded. After calling this method, the transaction will no longer be active.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there is no active transaction to roll back.
    /// </exception>
    void Rollback();
}