using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Data;
using RefactoringChallenge.Models;
using RefactoringChallenge.Services.Discounts;

namespace RefactoringChallenge.Tests;

[TestFixture]
public class DiscountCalculatorTests
{
    private ITimeProvider _timeProvider = null!;
    private DiscountCalculator _calculator = null!;

    [SetUp]
    public void Setup()
    {
        _timeProvider = new FixedTimeProvider(new DateTime(2025, 07, 02, 12, 00, 00));
        _calculator = new DiscountCalculator([
            new VipDiscountRule(),
            new LoyaltyDiscountRule(_timeProvider),
            new OrderAmountDiscountRule()
        ]);
    }

    [Test]
    public void ApplyDiscount_VipOldCustomerWithBigOrder_Max25Percent()
    {
        var customer = new Customer
        {
            IsVip = true,
            RegistrationDate = _timeProvider.Now.AddYears(-10)
        };

        var order = new Order
        {
            Items = [
                new OrderItem { Quantity = 2, UnitPrice = 6000 }
            ]
        };

        _calculator.ApplyDiscount(customer, order);

        Assert.That(order.DiscountPercent, Is.EqualTo(25));
        Assert.That(order.TotalAmount, Is.EqualTo(9000));
    }

    [Test]
    public void ApplyDiscount_RegularNewCustomerWithSmallOrder_NoDiscountAmount()
    {
        var customer = new Customer
        {
            IsVip = false,
            RegistrationDate = DateTime.Now
        };

        var order = new Order
        {
            Items = [
                new OrderItem { Quantity = 1, UnitPrice = 500 }
            ]
        };

        _calculator.ApplyDiscount(customer, order);

        Assert.That(order.DiscountPercent, Is.EqualTo(0));
        Assert.That(order.TotalAmount, Is.EqualTo(500));
    }

    [Test]
    public void ApplyDiscount_RegularCustomerWithMediumOrder_GetsAmountDiscountOnly()
    {
        var customer = new Customer
        {
            IsVip = false,
            RegistrationDate = _timeProvider.Now.AddYears(1)
        };

        var order = new Order
        {
            Items = [
                new OrderItem { Quantity = 1, UnitPrice = 6000 }
            ]
        };

        _calculator.ApplyDiscount(customer, order);

        Assert.That(order.DiscountPercent, Is.EqualTo(10));
        Assert.That(order.TotalAmount, Is.EqualTo(5400));
    }

    [Test]
    public void ApplyDiscount_EmptyItems_NoDiscountAmount()
    {
        var customer = new Customer
        {
            IsVip = true,
            RegistrationDate = _timeProvider.Now.AddYears(-5)
        };

        var order = new Order
        {
            Items = []
        };

        _calculator.ApplyDiscount(customer, order);

        Assert.That(order.DiscountAmount, Is.EqualTo(0));
        Assert.That(order.TotalAmount, Is.EqualTo(0));  
    }

    [Test]
    public void ApplyDiscount_NullItems_ThrowsInvalidOperationException()
    {
        var customer = new Customer
        {
            IsVip = false,
            RegistrationDate = _timeProvider.Now.AddYears(-1)
        };

        var order = new Order
        {
            Items = null!
        };

        Assert.Throws<InvalidOperationException>(() => _calculator.ApplyDiscount(customer, order));
    }
}
