using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Abstractions.Services;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Services;

/// <summary>
/// Provides functionality for processing customer orders, including handling pending orders
/// and coordinating with various repositories and services.
/// </summary>
/// <remarks>
/// This class is responsible for managing the lifecycle of customer orders, ensuring that
/// pending orders are processed correctly by utilizing the provided unit of work, repository
/// factory, and order processing service.
/// </remarks>
/// <param name="unitOfWorkFactory">
/// A factory for creating unit of work instances, which manage transactions and ensure
/// consistency during the order processing workflow.
/// </param>
/// <param name="repositoryFactory">
/// A factory for creating repository instances, such as customer, order, and inventory
/// repositories, which are used to interact with the underlying data storage.
/// </param>
/// <param name="orderProcessingService">
/// A service responsible for processing individual orders, ensuring that business rules
/// and validations are applied.
/// </param>
public class CustomerOrderProcessor(
    IUnitOfWorkFactory unitOfWorkFactory,
    IRepositoryFactory repositoryFactory,
    IOrderProcessingService orderProcessingService) : ICustomerOrderProcessor
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
    private readonly IRepositoryFactory _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
    private readonly IOrderProcessingService _orderProcessingService = orderProcessingService ?? throw new ArgumentNullException(nameof(orderProcessingService));

    /// <summary>
    /// Processes all pending orders for a specified customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer whose orders are to be processed.</param>
    /// <returns>A list of pending orders that were processed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="customerId"/> is less than or equal to zero.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the specified customer does not exist.</exception>
    /// <exception cref="Exception">Thrown when an error occurs during order processing, and the transaction is rolled back.</exception>
    public List<Order> ProcessCustomerOrders(int customerId)
    {
        if (customerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(customerId), "Customer ID must be a positive number.");

        using var uow = _unitOfWorkFactory.Create();
        uow.Begin();

        try
        {
            var customerRepository = _repositoryFactory.CreateCustomerRepository(uow);
            var orderRepository = _repositoryFactory.CreateOrderRepository(uow);
            var inventoryRepository = _repositoryFactory.CreateInventoryRepository(uow);

            var customer = customerRepository.GetById(customerId);
            var pendingOrders = orderRepository.GetPendingOrders(customerId);

            foreach (var order in pendingOrders)
            {
                _orderProcessingService.Process(customer, order, orderRepository, inventoryRepository);
            }

            uow.Commit();
            return pendingOrders;
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}