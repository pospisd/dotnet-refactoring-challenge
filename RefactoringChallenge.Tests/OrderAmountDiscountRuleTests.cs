using RefactoringChallenge.Models;
using RefactoringChallenge.Services.Discounts;

namespace RefactoringChallenge.Tests;

[TestFixture]
public class OrderAmountDiscountRuleTests
{
    [TestCase(10001, 15)]
    [TestCase(10000, 10)]
    [TestCase(5001, 10)]
    [TestCase(5000, 5)]
    [TestCase(1001, 5)]
    [TestCase(1000, 0)]
    [TestCase(0, 0)]
    [TestCase(-500, 0)]
    public void Apply_AddsCorrectOrderAmountDiscount(decimal totalAmount, decimal expectedDiscount)
    {
        var customer = new Customer();
        var order = new Order
        {
            Items = [new OrderItem { Quantity = 1, UnitPrice = totalAmount }]
        };

        var rule = new OrderAmountDiscountRule();
        var discount = rule.Calculate(customer, order);

        Assert.That(discount, Is.EqualTo(expectedDiscount));
    }
}