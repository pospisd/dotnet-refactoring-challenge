using RefactoringChallenge.Models;

namespace RefactoringChallenge.Abstractions.Data.Repositories;


/// <summary>
/// Represents a repository interface for managing and interacting with orders in the system.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Retrieves a list of pending orders for a specific customer.
    /// </summary>
    /// <param name="customerId">
    /// The unique identifier of the customer whose pending orders are to be retrieved.
    /// </param>
    /// <returns>
    /// A list of <see cref="Order"/> objects representing the pending orders for the specified customer.
    /// Each order includes its associated items and their respective product details.
    /// </returns>
    /// <remarks>
    /// This method queries the database to fetch orders with a status of 'Pending' for the given customer.
    /// </remarks>
    List<Order> GetPendingOrders(int customerId);

    /// <summary>
    /// Updates the details of the specified order in the system.
    /// </summary>
    /// <param name="order">The <see cref="Order"/> object containing updated information. 
    /// The <paramref name="order"/> must have a valid <see cref="Order.Id"/> and any modified properties 
    /// should reflect the desired changes.</param>
    /// <remarks>
    /// This method applies changes to the provided <paramref name="order"/> object in the underlying data store. 
    /// Ensure that the <paramref name="order"/> object is properly initialized and contains valid data 
    /// before invoking this method.
    /// </remarks>
    void UpdateOrder(Order order);

    /// <summary>
    /// Updates the status of the specified order.
    /// </summary>
    /// <param name="order">The <see cref="Order"/> object whose status is to be updated. 
    /// The <paramref name="order"/> must have a valid <see cref="Order.Id"/> and a non-null <see cref="Order.Status"/>.</param>
    /// <remarks>
    /// This method modifies the <see cref="Order.Status"/> property of the provided <paramref name="order"/> 
    /// to reflect the new status. Ensure that the <paramref name="order"/> object is properly initialized 
    /// before calling this method.
    /// </remarks>
    void UpdateStatus(Order order);

    /// <summary>
    /// Inserts a log entry associated with a specific order.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order for which the log entry is being created.</param>
    /// <param name="message">The log message describing the event or information to be recorded.</param>
    /// <remarks>
    /// This method is used to record important events or information related to an order. 
    /// Ensure that the <paramref name="orderId"/> corresponds to a valid order in the system.
    /// </remarks>
    void InsertLog(int orderId, string message);
}
