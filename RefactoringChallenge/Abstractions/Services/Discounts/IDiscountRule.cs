using RefactoringChallenge.Models;

namespace RefactoringChallenge.Abstractions.Services.Discounts;

/// <summary>
/// Defines a contract for implementing discount rules.
/// </summary>
/// <remarks>
/// Implementations of this interface provide logic to calculate discounts
/// based on specific criteria, such as customer attributes or order details.
/// </remarks>
public interface IDiscountRule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    decimal Calculate(Customer customer, Order order);
}