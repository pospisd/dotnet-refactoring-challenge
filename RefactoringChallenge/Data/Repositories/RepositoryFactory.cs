using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;

namespace RefactoringChallenge.Data.Repositories;

/// <summary>
/// 
/// </summary>
/// <param name="timeProvider"></param>
internal sealed class RepositoryFactory(ITimeProvider timeProvider) : IRepositoryFactory
{
    private readonly ITimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <summary>
    /// Creates an instance of <see cref="ICustomerRepository"/> for managing customer data.
    /// </summary>
    /// <param name="uow">The unit of work instance used to manage database connections and transactions.</param>
    /// <returns>An instance of <see cref="ICustomerRepository"/>.</returns>
    public ICustomerRepository CreateCustomerRepository(IUnitOfWork uow)
        => new CustomerRepository(uow);

    /// <summary>
    /// Creates an instance of <see cref="IOrderRepository"/> for managing order-related data.
    /// </summary>
    /// <param name="uow">The unit of work instance used to manage database transactions.</param>
    /// <returns>An implementation of <see cref="IOrderRepository"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="uow"/> is <c>null</c>.</exception>
    public IOrderRepository CreateOrderRepository(IUnitOfWork uow)
        => new OrderRepository(uow, _timeProvider);

    /// <summary>
    /// Creates an instance of <see cref="IInventoryRepository"/> for managing inventory-related operations.
    /// </summary>
    /// <param name="uow">
    /// The unit of work instance (<see cref="IUnitOfWork"/>) that provides database connection and transaction context.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IInventoryRepository"/> for performing inventory operations such as stock checks and inventory reduction.
    /// </returns>
    public IInventoryRepository CreateInventoryRepository(IUnitOfWork uow)
        => new InventoryRepository(uow);
}