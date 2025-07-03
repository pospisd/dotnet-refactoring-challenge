using RefactoringChallenge.Abstractions.Services.Discounts;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Services.Discounts;

/// <summary>
/// Represents a discount rule specifically designed for VIP customers.
/// </summary>
/// <remarks>
/// This rule applies a fixed discount percentage to orders placed by customers
/// who are marked as VIPs. The discount is determined based on the customer's
/// VIP status.
/// </remarks>
internal sealed class VipDiscountRule : IDiscountRule
{
    /// <summary>
    /// Calculates the discount percentage for a given order based on the customer's VIP status.
    /// </summary>
    /// <param name="customer">The customer placing the order. The customer's VIP status determines the discount.</param>
    /// <param name="order">The order for which the discount is being calculated. This parameter is required but not directly used in the calculation.</param>
    /// <returns>
    /// A <see cref="decimal"/> value representing the discount percentage. 
    /// Returns 10 if the customer is a VIP; otherwise, returns 0.
    /// </returns>
    public decimal Calculate(Customer customer, Order order) =>
        customer.IsVip ? 10 : 0;
}