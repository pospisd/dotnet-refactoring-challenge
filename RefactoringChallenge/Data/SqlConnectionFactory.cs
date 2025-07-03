using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Configuration;

namespace RefactoringChallenge.Data;

internal sealed class SqlConnectionFactory(IOptions<DbSettings> options) : IDbConnectionFactory
{
    private readonly string _connectionString = options.Value.ConnectionString
                                                ?? throw new ArgumentNullException(nameof(options.Value.ConnectionString));

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}