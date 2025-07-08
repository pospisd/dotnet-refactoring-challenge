using Moq;
using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Abstractions.Services;
using RefactoringChallenge.Abstractions.Services.Discounts;
using RefactoringChallenge.Models;
using RefactoringChallenge.Services;

namespace RefactoringChallenge.Tests;

[TestFixture]
public class OrderProcessingServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IInventoryRepository> _inventoryRepositoryMock = null!;
    private Mock<IDiscountCalculator> _discountCalculatorMock = null!;
    private IOrderProcessingService _orderProcessingService = null!;

    [SetUp]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _discountCalculatorMock = new Mock<IDiscountCalculator>();
        _orderProcessingService = new OrderProcessingService(_discountCalculatorMock.Object);
    }

    [Test]
    public void Process_OrderReady_SetsStatusToReady()
    {
        var customer = new Customer();
        var order = new Order
        {
            Id = 1,
            Items = [],
            DiscountPercent = 10,
            TotalAmount = 1000
        };

        _inventoryRepositoryMock.Setup(i => i.AreAllItemsInStock(order.Id)).Returns(true);

        _orderProcessingService.Process(customer, order, _orderRepositoryMock.Object, _inventoryRepositoryMock.Object);

        Assert.That(order.Status, Is.EqualTo("Ready"));
    }

    [Test]
    public void Process_OrderReady_LogsCompletion()
    {
        var customer = new Customer();
        var order = new Order
        {
            Id = 2,
            Items = [],
            DiscountPercent = 10,
            TotalAmount = 1000
        };

        _inventoryRepositoryMock.Setup(i => i.AreAllItemsInStock(order.Id)).Returns(true);

        _orderProcessingService.Process(customer, order, _orderRepositoryMock.Object, _inventoryRepositoryMock.Object);

        _orderRepositoryMock.Verify(r => r.InsertLog(order.Id,
            It.Is<string>(msg => msg.Contains("completed") && msg.Contains("discount"))), Times.Once);
    }

    [Test]
    public void Process_OrderNotInStock_SetsStatusToOnHold()
    {
        var customer = new Customer();
        var order = new Order
        {
            Id = 3,
            Items = []
        };

        _inventoryRepositoryMock.Setup(i => i.AreAllItemsInStock(order.Id)).Returns(false);

        _orderProcessingService.Process(customer, order, _orderRepositoryMock.Object, _inventoryRepositoryMock.Object);

        Assert.That(order.Status, Is.EqualTo("OnHold"));
    }

    [Test]
    public void Process_OrderNotInStock_LogsHoldMessage()
    {
        var customer = new Customer();
        var order = new Order
        {
            Id = 4,
            Items = []
        };

        _inventoryRepositoryMock.Setup(i => i.AreAllItemsInStock(order.Id)).Returns(false);

        _orderProcessingService.Process(customer, order, _orderRepositoryMock.Object, _inventoryRepositoryMock.Object);

        _orderRepositoryMock.Verify(r => r.InsertLog(order.Id,
            It.Is<string>(msg => msg.Contains("on hold"))), Times.Once);
    }

    [Test]
    public void Process_OrderReady_CallsReduceInventory()
    {
        var customer = new Customer();
        var order = new Order
        {
            Id = 5,
            Items = []
        };

        _inventoryRepositoryMock.Setup(i => i.AreAllItemsInStock(order.Id)).Returns(true);

        _orderProcessingService.Process(customer, order, _orderRepositoryMock.Object, _inventoryRepositoryMock.Object);

        _inventoryRepositoryMock.Verify(i => i.ReduceInventory(order.Id), Times.Once);
    }

    [Test]
    public void Process_OrderNotInStock_DoesNotCallReduceInventory()
    {
        var customer = new Customer();
        var order = new Order
        {
            Id = 6,
            Items = []
        };

        _inventoryRepositoryMock.Setup(i => i.AreAllItemsInStock(order.Id)).Returns(false);
        
        _orderProcessingService.Process(customer, order, _orderRepositoryMock.Object, _inventoryRepositoryMock.Object);
        
        _inventoryRepositoryMock.Verify(i => i.ReduceInventory(It.IsAny<int>()), Times.Never);
    }
}
