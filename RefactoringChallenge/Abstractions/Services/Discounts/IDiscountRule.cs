using RefactoringChallenge.Models;

namespace RefactoringChallenge.Abstractions.Services.Discounts;

public interface IDiscountRule
{
    decimal Calculate(Customer customer, Order order);
}