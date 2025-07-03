using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Services.Discounts;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Services.Discounts;

/// <summary>
/// Represents a discount rule that calculates a discount percentage based on customer loyalty.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IDiscountRule"/> interface and uses an <see cref="ITimeProvider"/> 
/// to determine the current date for calculating the duration of a customer's loyalty.
/// </remarks>
public class LoyaltyDiscountRule(ITimeProvider timeProvider) : IDiscountRule
{
    /// <summary>
    /// Calculates the discount percentage for a customer based on their loyalty.
    /// </summary>
    /// <param name="customer">The customer for whom the discount is being calculated.</param>
    /// <param name="order">The order associated with the customer.</param>
    /// <returns>
    /// A decimal value representing the discount percentage:
    /// - 5% for customers registered for 5 or more years.
    /// - 2% for customers registered for 2 or more years.
    /// - 0% for customers registered for less than 2 years.
    /// </returns>
    public decimal Calculate(Customer customer, Order order)
    {
        var years = timeProvider.Now.Year - customer.RegistrationDate.Year;
            
        return years switch
        {
            >= 5 => 5,
            >= 2 => 2,
            _ => 0
        };
    }
}