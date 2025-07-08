using RefactoringChallenge.Abstractions.Services.Discounts;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Services.Discounts;

/// <summary>
/// Represents a discount rule based on the total amount of an order.
/// </summary>
/// <remarks>
/// This rule calculates a discount percentage based on the total value of the order.
/// The discount tiers are as follows:
/// - Orders greater than 10,000 receive a 15% discount.
/// - Orders greater than 5,000 receive a 10% discount.
/// - Orders greater than 1,000 receive a 5% discount.
/// - Orders below or equal to 1,000 receive no discount.
/// </remarks>
internal sealed class OrderAmountDiscountRule : IDiscountRule
{
    /// <summary>
    /// Calculates the discount percentage for a given order based on its total amount.
    /// </summary>
    /// <param name="customer">
    /// The customer placing the order. This parameter is required but not directly used in the calculation.
    /// </param>
    /// <param name="order">
    /// The order for which the discount is being calculated. The total amount of the order determines the discount percentage.
    /// </param>
    /// <returns>
    /// A <see cref="decimal"/> value representing the discount percentage. 
    /// The discount is determined as follows:
    /// - Orders greater than 10,000 receive a 15% discount.
    /// - Orders greater than 5,000 receive a 10% discount.
    /// - Orders greater than 1,000 receive a 5% discount.
    /// - Orders below or equal to 1,000 receive no discount.
    /// </returns>
    public decimal Calculate(Customer customer, Order order)
    {
        var amount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        return amount switch
        {
            > 10_000 => 15,
            > 5_000 => 10,
            > 1_000 => 5,
            _ => 0
        };
    }
}