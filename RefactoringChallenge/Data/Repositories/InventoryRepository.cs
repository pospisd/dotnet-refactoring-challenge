using System.Data;
using Dapper;
using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;

namespace RefactoringChallenge.Data.Repositories;

/// <summary>
/// Represents a repository for managing inventory-related operations.
/// Provides methods to check stock availability and update inventory levels
/// based on order details.
/// </summary>
internal class InventoryRepository : IInventoryRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryRepository"/> class.
    /// </summary>
    /// <param name="unitOfWork">
    /// The unit of work that provides the database connection and transaction context 
    /// for executing inventory-related operations.
    /// </param>
    /// <remarks>
    /// The <paramref name="unitOfWork"/> is used to ensure that all operations performed 
    /// by this repository are executed within the same transaction and database context.
    /// </remarks>
    public InventoryRepository(IUnitOfWork unitOfWork)
    {
        _connection = unitOfWork.Connection;
        _transaction = unitOfWork.Transaction;
    }

    /// <summary>
    /// Determines whether all items in a specified order are in stock.
    /// </summary>
    /// <param name="orderId">
    /// The unique identifier of the order to check for stock availability.
    /// </param>
    /// <returns>
    /// <c>true</c> if all items in the order are in stock; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method queries the database to verify if the stock quantity for each item
    /// in the specified order is sufficient. If any item is out of stock or has insufficient
    /// quantity, the method returns <c>false</c>.
    /// </remarks>
    public bool AreAllItemsInStock(int orderId)
    {
        const string sql = """
                           SELECT COUNT(*) 
                           FROM OrderItems oi
                           LEFT JOIN Inventory i ON oi.ProductId = i.ProductId
                           WHERE oi.OrderId = @OrderId
                             AND (i.StockQuantity IS NULL OR i.StockQuantity < oi.Quantity)
                           """;

        var insufficientCount = _connection.ExecuteScalar<int>(
            sql,
            new { OrderId = orderId },
            transaction: _transaction);

        return insufficientCount == 0;
    }

    public void ReduceInventory(int orderId)
    {
        const string sql = """
                           UPDATE i
                           SET i.StockQuantity = i.StockQuantity - oi.Quantity
                           FROM Inventory i
                           INNER JOIN OrderItems oi ON oi.ProductId = i.ProductId
                           WHERE oi.OrderId = @OrderId
                           """;

        using var command = _connection.CreateCommand();
        command.Transaction = _transaction;
        command.CommandText = sql;

        var param = command.CreateParameter();
        param.ParameterName = "@OrderId";
        param.Value = orderId;
        command.Parameters.Add(param);

        command.ExecuteNonQuery();
    }
}