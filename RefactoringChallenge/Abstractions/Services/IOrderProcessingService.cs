using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Abstractions.Services;

/// <summary>
/// Defines the contract for processing customer orders, including applying discounts,
/// updating order statuses, and managing inventory. This interface ensures that orders
/// are processed efficiently and consistently.
/// </summary>
/// <remarks>
/// Implementations of this interface are expected to interact with various repositories
/// and services, such as <see cref="IOrderRepository"/> and <see cref="IInventoryRepository"/>,
/// to perform operations like updating inventory, applying discounts, and logging actions.
/// </remarks>
public interface IOrderProcessingService
{
    /// <summary>
    /// Processes a customer's order by applying discounts, updating the order status,
    /// and managing inventory.
    /// </summary>
    /// <param name="customer">
    /// The customer associated with the order. Contains details such as customer ID, name,
    /// email, and VIP status.
    /// </param>
    /// <param name="order">
    /// The order to be processed. Includes details such as order ID, customer ID, order date,
    /// total amount, discount information, status, and items.
    /// </param>
    /// <param name="orderRepository">
    /// The repository used for interacting with order data, such as updating orders,
    /// changing statuses, and logging actions.
    /// </param>
    /// <param name="inventoryRepository">
    /// The repository used for managing inventory operations, such as checking stock
    /// availability and reducing inventory.
    /// </param>
    /// <remarks>
    /// This method ensures that the order is processed efficiently by applying any applicable
    /// discounts, verifying inventory availability, and updating the order's status. It relies
    /// on the provided repositories to perform these operations.
    /// </remarks>
    void Process(Customer customer, Order order, IOrderRepository orderRepository, IInventoryRepository inventoryRepository);
}