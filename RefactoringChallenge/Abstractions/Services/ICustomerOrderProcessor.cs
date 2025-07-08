using RefactoringChallenge.Models;

namespace RefactoringChallenge.Abstractions.Services;

/// <summary>
/// Defines the contract for processing customer orders, including handling pending orders
/// and ensuring proper transaction management during the process.
/// </summary>
public interface ICustomerOrderProcessor
{
    /// <summary>
    /// Processes all pending orders for a specified customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer whose orders are to be processed.</param>
    /// <returns>A list of pending orders that were processed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="customerId"/> is less than or equal to zero.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the specified customer does not exist.</exception>
    /// <exception cref="Exception">Thrown when an error occurs during order processing, and the transaction is rolled back.</exception>
    List<Order> ProcessCustomerOrders(int customerId);
}