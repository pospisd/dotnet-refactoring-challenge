using RefactoringChallenge.Models;

namespace RefactoringChallenge.Abstractions.Data.Repositories;

/// <summary>
/// Provides an abstraction for inventory-related operations, including checking stock availability
/// and reducing inventory based on order items.
/// </summary>
public interface IInventoryRepository
{
    /// <summary>
    /// Determines whether all items in the specified order are currently in stock.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to check.</param>
    /// <returns>
    /// <c>true</c> if all items in the order are in stock; otherwise, <c>false</c>.
    /// </returns>
    bool AreAllItemsInStock(int orderId);

    /// <summary>
    /// Reduces the inventory quantities for all items in the specified order.
    /// </summary>
    /// <param name="orderId">
    /// The unique identifier of the order whose items' inventory quantities should be reduced.
    /// </param>
    /// <remarks>
    /// This method updates the stock quantities in the inventory based on the quantities
    /// specified in the order. It assumes that the order items and inventory are properly
    /// linked and that the stock quantities are sufficient for the reduction.
    /// </remarks>
    void ReduceInventory(int orderId);
}