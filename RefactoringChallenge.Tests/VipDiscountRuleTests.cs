using RefactoringChallenge.Models;
using RefactoringChallenge.Services.Discounts;

namespace RefactoringChallenge.Tests;

[TestFixture]
public class VipDiscountRuleTests
{
    [TestCase(true, 10)]
    [TestCase(false, 0)]
    public void Apply_AddsCorrectVipDiscount(bool isVip, decimal expectedDiscount)
    {
        var customer = new Customer { IsVip = isVip };
        var order = new Order
        {
            Items = [new OrderItem { Quantity = 1, UnitPrice = 1000 }]
        };

        var rule = new VipDiscountRule();
        var discount = rule.Calculate(customer, order);

        Assert.That(discount, Is.EqualTo(expectedDiscount));
    }
}