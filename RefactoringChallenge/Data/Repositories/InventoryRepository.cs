using System.Data;
using Dapper;
using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Data.Repositories;

internal class InventoryRepository : IInventoryRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public InventoryRepository(IUnitOfWork unitOfWork)
    {
        _connection = unitOfWork.Connection;
        _transaction = unitOfWork.Transaction;
    }

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