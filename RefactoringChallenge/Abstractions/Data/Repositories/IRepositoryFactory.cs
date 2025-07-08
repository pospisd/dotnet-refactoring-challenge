namespace RefactoringChallenge.Abstractions.Data.Repositories;

/// <summary>
/// Defines a factory for creating repository instances.
/// </summary>
/// <remarks>
/// This interface provides methods for creating specific repository instances, 
/// such as customer and order repositories, which are used to interact with 
/// the underlying data storage.
/// </remarks>
public interface IRepositoryFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ICustomerRepository"/> for managing customer data.
    /// </summary>
    /// <param name="uow">
    /// An instance of <see cref="IUnitOfWork"/> that provides the transactional context 
    /// for the repository operations.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="ICustomerRepository"/> for accessing and managing customer data.
    /// </returns>
    ICustomerRepository CreateCustomerRepository(IUnitOfWork uow);
    
    /// <summary>
    /// Creates an instance of <see cref="IOrderRepository"/> for managing and interacting with orders.
    /// </summary>
    /// <param name="uow">
    /// The <see cref="IUnitOfWork"/> instance that provides the transactional context for the repository.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IOrderRepository"/> for managing orders.
    /// </returns>
    IOrderRepository CreateOrderRepository(IUnitOfWork uow);

    /// <summary>
    /// Creates an instance of <see cref="IInventoryRepository"/> for managing inventory-related operations.
    /// </summary>
    /// <param name="uow">
    /// An instance of <see cref="IUnitOfWork"/> that provides the transactional context 
    /// for the repository operations.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="IInventoryRepository"/> for accessing and managing inventory data.
    /// </returns>
    IInventoryRepository CreateInventoryRepository(IUnitOfWork uow);
}