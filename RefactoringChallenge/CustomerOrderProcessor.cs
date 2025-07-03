using RefactoringChallenge.Abstractions;
using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Models;

namespace RefactoringChallenge;

/// <summary>
/// Handles the processing of customer orders, including retrieving pending orders,
/// loading associated data, and applying necessary processing logic.
/// </summary>
/// <remarks>
/// This class utilizes dependency injection to manage its dependencies, including
/// time-related operations, unit of work management, and repository creation.
/// </remarks>
/// <param name="timeProvider">
/// Provides the current time, enabling time-dependent operations.
/// </param>
/// <param name="unitOfWorkFactory">
/// Factory for creating unit of work instances to manage database transactions.
/// </param>
/// <param name="repositoryFactory">
/// Factory for creating repository instances for accessing customer and order data.
/// </param>
public class CustomerOrderProcessor(
    ITimeProvider timeProvider,
    IUnitOfWorkFactory unitOfWorkFactory,
    IRepositoryFactory repositoryFactory)
{
    private readonly ITimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
    private readonly IRepositoryFactory _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));

    /// <summary>
    /// Processes all pending orders for a specified customer, including loading associated data
    /// and applying necessary processing logic.
    /// </summary>
    /// <param name="customerId">
    /// The unique identifier of the customer whose orders are to be processed. Must be a positive integer.
    /// </param>
    /// <returns>
    /// A list of processed orders for the specified customer.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="customerId"/> is less than or equal to zero.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified customer does not exist in the system.
    /// </exception>
    /// <remarks>
    /// This method retrieves pending orders for the specified customer, loads associated data such as
    /// order items and products, and processes each order. The operation is transactional, ensuring
    /// that changes are either fully committed or rolled back in case of an error.
    /// </remarks>
    public List<Order> ProcessCustomerOrders(int customerId)
    {
        if (customerId <= 0)
            throw new ArgumentException("ID zákazníka musí být kladné číslo.", nameof(customerId));

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
                ProcessOrder(orderRepository,inventoryRepository, customer, order);
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

    private void ProcessOrder(
        IOrderRepository orderRepository, 
        IInventoryRepository inventoryRepository, 
        Customer customer, 
        Order order)
    {
        CalculateDiscount(customer, order);
        orderRepository.UpdateOrder(order);

        if (inventoryRepository.AreAllItemsInStock(order.Id))
        {
            inventoryRepository.ReduceInventory(order.Id);
            order.Status = "Ready";
            orderRepository.UpdateStatus(order);
            orderRepository.InsertLog(order.Id, $"Order completed with {order.DiscountPercent}% discount. Total price: {order.TotalAmount}");
        }
        else
        {
            order.Status = "OnHold";
            orderRepository.UpdateStatus(order);
            orderRepository.InsertLog(order.Id, "Order on hold. Some items are not on stock.");
        }
    }

    private void CalculateDiscount(Customer customer, Order order)
    {
        decimal totalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);
        decimal discountPercent = 0;

        if (customer.IsVip) discountPercent += 10;

        int years = _timeProvider.Now.Year - customer.RegistrationDate.Year;
        if (years >= 5) discountPercent += 5;
        else if (years >= 2) discountPercent += 2;

        if (totalAmount > 10000) discountPercent += 15;
        else if (totalAmount > 5000) discountPercent += 10;
        else if (totalAmount > 1000) discountPercent += 5;

        if (discountPercent > 25) discountPercent = 25;

        var discountAmount = totalAmount * (discountPercent / 100);
        var finalAmount = totalAmount - discountAmount;

        order.DiscountPercent = discountPercent;
        order.DiscountAmount = discountAmount;
        order.TotalAmount = finalAmount;
        order.Status = "Processed";
    }
}


