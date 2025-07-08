using RefactoringChallenge.Abstractions.Services.Discounts;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Services.Discounts;

/// <summary>
/// Represents a service for calculating discounts for customer orders based on a set of discount rules.
/// </summary>
/// <remarks>
/// This class applies a series of discount rules to calculate the total discount percentage and amount for a given order.
/// The discount percentage is capped at a maximum of 25% to ensure limits on the applied discounts.
/// </remarks>
public class DiscountCalculator(IEnumerable<IDiscountRule> rules) : IDiscountCalculator
{
    private readonly List<IDiscountRule> _rules = rules.ToList();

    /// <summary>
    /// Applies discount rules to a customer's order, calculating and updating the discount percentage, 
    /// discount amount, and final total amount of the order.
    /// </summary>
    /// <param name="customer">The customer for whom the discount is being applied.</param>
    /// <param name="order">The order to which the discount is applied.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the order's items are not loaded or are null.
    /// </exception>
    /// <remarks>
    /// The discount percentage is calculated based on the provided discount rules and is capped at a maximum of 25%. 
    /// The method updates the order with the calculated discount percentage, discount amount, and final total amount. 
    /// Additionally, the order's status is updated to "Processed".
    /// </remarks>
    public void ApplyDiscount(Customer customer, Order order)
    {
        if (order.Items == null) throw new InvalidOperationException("Order items must be loaded");

        var totalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        var discountPercent = _rules.Sum(rule => rule.Calculate(customer, order));
        if (discountPercent > 25) discountPercent = 25;

        var discountAmount = totalAmount * (discountPercent / 100m);
        var finalAmount = totalAmount - discountAmount;

        order.DiscountPercent = discountPercent;
        order.DiscountAmount = discountAmount;
        order.TotalAmount = finalAmount;
        order.Status = "Processed";
    }
}