using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;

namespace RefactoringChallenge.Data.Repositories;

internal sealed class RepositoryFactory(ITimeProvider timeProvider) : IRepositoryFactory
{
    private readonly ITimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    public ICustomerRepository CreateCustomerRepository(IUnitOfWork uow)
        => new CustomerRepository(uow);

    public IOrderRepository CreateOrderRepository(IUnitOfWork uow)
        => new OrderRepository(uow, _timeProvider);

    public IInventoryRepository CreateInventoryRepository(IUnitOfWork uow)
        => new InventoryRepository(uow);
}