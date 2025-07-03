using Microsoft.Data.SqlClient;
using RefactoringChallenge.Models;

namespace RefactoringChallenge.Abstractions.Data.Repositories;

/// <summary>
/// Provides an abstraction for accessing and managing customer data.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Retrieves a customer by their unique identifier.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer to retrieve.</param>
    /// <returns>The <see cref="Customer"/> object corresponding to the specified identifier.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no customer with the specified identifier is found.
    /// </exception>
    Customer GetById(int customerId);
}