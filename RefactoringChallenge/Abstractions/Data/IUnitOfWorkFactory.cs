namespace RefactoringChallenge.Abstractions.Data;

/// <summary>
/// Defines a factory for creating instances of <see cref="IUnitOfWork"/>.
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IUnitOfWork"/>.
    /// </summary>
    /// <remarks>
    /// The created <see cref="IUnitOfWork"/> instance provides a transactional context 
    /// for performing a set of operations as a single unit of work. 
    /// Ensure to properly dispose of the returned instance to release resources.
    /// </remarks>
    /// <returns>
    /// A new instance of <see cref="IUnitOfWork"/>.
    /// </returns>
    IUnitOfWork Create();
}