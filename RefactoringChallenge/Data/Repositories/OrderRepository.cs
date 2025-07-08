using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Models;
using System.Data;
using Dapper;

namespace RefactoringChallenge.Data.Repositories;

/// <summary>
/// Provides an implementation of <see cref="IOrderRepository"/> for managing and interacting with orders in the system.
/// </summary>
/// <remarks>
/// This repository utilizes Dapper for database operations and depends on an <see cref="IUnitOfWork"/> for managing database connections and transactions.
/// It also uses an <see cref="ITimeProvider"/> to handle time-related operations.
/// </remarks>
/// <param name="unitOfWork">The unit of work providing the database connection and transaction context.</param>
/// <param name="timeProvider">The time provider used for retrieving the current time.</param>
internal class OrderRepository(IUnitOfWork unitOfWork, ITimeProvider timeProvider) : IOrderRepository
{
    private readonly IDbConnection _connection = unitOfWork.Connection;
    private readonly IDbTransaction? _transaction = unitOfWork.Transaction;
    private readonly ITimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <summary>
    /// Retrieves a list of pending orders for a specific customer.
    /// </summary>
    /// <param name="customerId">
    /// The unique identifier of the customer whose pending orders are to be retrieved.
    /// </param>
    /// <returns>
    /// A list of <see cref="Order"/> objects representing the pending orders for the specified customer.
    /// Each order includes its associated items and their respective product details.
    /// </returns>
    /// <remarks>
    /// This method queries the database to fetch orders with a status of 'Pending' for the given customer.
    /// It uses Dapper to map the results into <see cref="Order"/>, <see cref="OrderItem"/>, and <see cref="Product"/> objects.
    /// </remarks>
    public List<Order> GetPendingOrders(int customerId)
    {
        const string sql = """
                           SELECT 
                               o.Id, o.CustomerId, o.OrderDate, o.TotalAmount, o.Status,
                               oi.Id, oi.OrderId, oi.ProductId, oi.Quantity, oi.UnitPrice,
                               p.Id, p.Name, p.Category, p.Price
                           FROM Orders o
                           INNER JOIN OrderItems oi ON o.Id = oi.OrderId
                           INNER JOIN Products p ON oi.ProductId = p.Id
                           WHERE o.CustomerId = @CustomerId AND o.Status = 'Pending'
                           """;

        var orderDictionary = new Dictionary<int, Order>();

        _connection.Query<Order, OrderItem, Product, Order>(
            sql,
            (order, item, product) =>
            {
                if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
                {
                    currentOrder = order;
                    currentOrder.Items = [];
                    orderDictionary.Add(order.Id, currentOrder);
                }

                item.Product = product;
                currentOrder.Items.Add(item);

                return currentOrder;
            },
            new { CustomerId = customerId },
            transaction: _transaction,
            splitOn: "Id,Id"
        );

        return orderDictionary.Values.ToList();
    }

    /// <summary>
    /// Updates the details of an existing order in the database.
    /// </summary>
    /// <param name="order">The <see cref="Order"/> object containing the updated order details.</param>
    /// <remarks>
    /// This method updates the order's total amount, discount percentage, discount amount, and status.
    /// The changes are applied to the database using the current transaction context, if available.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="order"/> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the database connection is not open or if the update operation fails.
    /// </exception>
    public void UpdateOrder(Order order)
    {
        const string sql = """
                           UPDATE Orders 
                           SET TotalAmount = @TotalAmount, 
                               DiscountPercent = @DiscountPercent, 
                               DiscountAmount = @DiscountAmount, 
                               Status = @Status 
                           WHERE Id = @OrderId
                           """;

        _connection.Execute(
            sql,
            new
            {
                order.TotalAmount,
                order.DiscountPercent,
                order.DiscountAmount,
                order.Status,
                OrderId = order.Id
            },
            transaction: _transaction);
    }

    /// <summary>
    /// Updates the status of the specified order in the database.
    /// </summary>
    /// <param name="order">The order whose status needs to be updated. The <see cref="Order.Status"/> property should contain the new status.</param>
    /// <remarks>
    /// This method executes an SQL update query to modify the status of the order identified by its <see cref="Order.Id"/>.
    /// </remarks>
    public void UpdateStatus(Order order)
    {
        const string sql = "UPDATE Orders SET Status = @Status WHERE Id = @OrderId";

        _connection.Execute(
            sql,
            new { order.Status, OrderId = order.Id },
            transaction: _transaction);
    }

    /// <summary>
    /// Inserts a log entry for a specific order into the database.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order for which the log entry is being created.</param>
    /// <param name="message">The log message to be recorded.</param>
    /// <remarks>
    /// This method records the log entry with the current date and time provided by the <see cref="ITimeProvider"/>.
    /// </remarks>
    public void InsertLog(int orderId, string message)
    {
        const string sql = """
                           INSERT INTO OrderLogs (OrderId, LogDate, Message) 
                           VALUES (@OrderId, @LogDate, @Message)
                           """;

        _connection.Execute(
            sql,
            new
            {
                OrderId = orderId,
                LogDate = _timeProvider.Now,
                Message = message
            },
            transaction: _transaction);
    }
}