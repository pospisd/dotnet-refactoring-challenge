using RefactoringChallenge.Models;


namespace RefactoringChallenge.Abstractions.Services.Discounts
{
    public interface IDiscountCalculator
    {
        void ApplyDiscount(Customer customer, Order order);
    }
}
