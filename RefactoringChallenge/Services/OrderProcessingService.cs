using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Abstractions.Services;
using RefactoringChallenge.Abstractions.Services.Discounts;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Services;

/// <summary>
/// Provides functionality for processing customer orders, including applying discounts,
/// updating order statuses, and managing inventory. This service ensures that orders are
/// processed efficiently and logs relevant actions for tracking purposes.
/// </summary>
/// <remarks>
/// This service interacts with various repositories and services, such as 
/// <see cref="IDiscountCalculator"/>, <see cref="IOrderRepository"/>, and <see cref="IInventoryRepository"/>,
/// to perform its operations. It ensures that discounts are applied, inventory is updated,
/// and order statuses are set appropriately based on stock availability.
/// </remarks>
internal sealed class OrderProcessingService(IDiscountCalculator discountCalculator) : IOrderProcessingService
{
    private readonly IDiscountCalculator _discountCalculator = discountCalculator ?? throw new ArgumentNullException(nameof(discountCalculator));

    /// <summary>
    /// Processes a customer order by applying discounts, updating the order status,
    /// and managing inventory. Logs relevant actions for tracking purposes.
    /// </summary>
    /// <param name="customer">
    /// The customer associated with the order. Used to determine applicable discounts.
    /// </param>
    /// <param name="order">
    /// The order to be processed. Includes details such as items, total amount, and discount information.
    /// </param>
    /// <param name="orderRepository">
    /// The repository used to update the order and log processing actions.
    /// </param>
    /// <param name="inventoryRepository">
    /// The repository used to check stock availability and reduce inventory for the order.
    /// </param>
    /// <remarks>
    /// This method ensures that discounts are applied to the order, inventory is updated based on stock availability,
    /// and the order status is set to either "Ready" or "OnHold". Logs are inserted to track the processing outcome.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if any of the parameters <paramref name="customer"/>, <paramref name="order"/>,
    /// <paramref name="orderRepository"/>, or <paramref name="inventoryRepository"/> is <c>null</c>.
    /// </exception>
    public void Process(Customer customer, Order order, IOrderRepository orderRepository, IInventoryRepository inventoryRepository)
    {
        _discountCalculator.ApplyDiscount(customer, order);
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
}