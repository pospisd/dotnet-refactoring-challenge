using System.Data;
using Dapper;
using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Data.Repositories;

/// <summary>
/// Represents a repository for managing customer data, providing methods to interact with the underlying data store.
/// </summary>
/// <remarks>
/// This repository is implemented using Dapper for data access and relies on an <see cref="IUnitOfWork"/> 
/// to manage database connections and transactions. It provides functionality to retrieve customer information 
/// by their unique identifier.
/// </remarks>
internal class CustomerRepository(IUnitOfWork unitOfWork) : ICustomerRepository
{
    private readonly IDbConnection _connection = unitOfWork.Connection;
    private readonly IDbTransaction? _transaction = unitOfWork.Transaction;

    /// <summary>
    /// Retrieves a customer by their unique identifier.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer to retrieve.</param>
    /// <returns>
    /// An instance of <see cref="Customer"/> representing the customer with the specified identifier.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no customer with the specified <paramref name="customerId"/> is found in the data store.
    /// </exception>
    public Customer GetById(int customerId)
    {
        const string sql = """
                           SELECT Id, Name, Email, IsVip, RegistrationDate 
                           FROM Customers 
                           WHERE Id = @CustomerId
                           """;

        var customer = _connection.QuerySingleOrDefault<Customer>(
            sql,
            new { CustomerId = customerId },
            transaction: _transaction
        );

        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {customerId} not found.");

        return customer;
    }
}