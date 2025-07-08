using RefactoringChallenge.Models;


namespace RefactoringChallenge.Abstractions.Services.Discounts;

/// <summary>
/// Defines the contract for a service that calculates and applies discounts to customer orders.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for determining applicable discounts
/// based on customer and order details, and applying those discounts to the order.
/// </remarks>
public interface IDiscountCalculator
{
    /// <summary>
    /// Applies a discount to the specified order based on the customer's details and the order's contents.
    /// </summary>
    /// <param name="customer">
    /// The <see cref="Customer"/> object containing details about the customer, such as their VIP status
    /// and registration date, which may influence the discount calculation.
    /// </param>
    /// <param name="order">
    /// The <see cref="Order"/> object representing the customer's order, including its items, total amount,
    /// and discount-related properties to be updated.
    /// </param>
    /// <remarks>
    /// This method calculates the applicable discount percentage based on predefined rules, ensures that
    /// the discount does not exceed a maximum threshold, and updates the order's discount percentage,
    /// discount amount, total amount, and status accordingly.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the order's items are not loaded or are null, as the discount calculation requires
    /// item details.
    /// </exception>
    void ApplyDiscount(Customer customer, Order order);
}