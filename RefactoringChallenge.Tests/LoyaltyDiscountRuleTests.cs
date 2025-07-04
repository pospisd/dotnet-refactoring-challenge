using RefactoringChallenge.Data;
using RefactoringChallenge.Models;
using RefactoringChallenge.Services.Discounts;

namespace RefactoringChallenge.Tests;

[TestFixture]
public class LoyaltyDiscountRuleTests
{
    [TestCase(6, 5)]
    [TestCase(5, 5)]
    [TestCase(3, 2)]
    [TestCase(1, 0)]
    [TestCase(0, 0)]
    [TestCase(-1, 0)]
    public void Apply_AddsCorrectLoyaltyDiscount(int yearsSinceRegistration, decimal expectedDiscount)
    {
        var timeProvider = new FixedTimeProvider(new DateTime(2025, 07, 02, 12, 00, 00));
        var registrationDate = timeProvider.Now.AddYears(-yearsSinceRegistration);
        var customer = new Customer { RegistrationDate = registrationDate };
        var order = new Order { Items = [new OrderItem { Quantity = 1, UnitPrice = 1000 }] };
        

        var rule = new LoyaltyDiscountRule(timeProvider);
        var discount = rule.Calculate(customer, order);

        Assert.That(discount, Is.EqualTo(expectedDiscount));
    }
}