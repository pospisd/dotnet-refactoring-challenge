using Newtonsoft.Json;
using Testcontainers.MsSql;
using Microsoft.Data.SqlClient;
using RefactoringChallenge.Tests.Snapshot.Providers;

namespace RefactoringChallenge.Tests.Snapshot;

[TestFixture]
public class SnapshotTests
{
    private string _snapshotFolder = null!;
    private string _connectionString = null!;
    private MsSqlContainer _dbContainer = null!;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {

        _dbContainer = new MsSqlBuilder().Build();
        await _dbContainer.StartAsync();

        await _dbContainer.StartAsync();

        _connectionString = _dbContainer.GetConnectionString();

        await ExecuteSqlScript("Data/DatabaseSchema.sql");

        _connectionString += ";Database = refactoringchallenge;";
        _snapshotFolder = Path.Combine(GetProjectRootPath(),"Snapshots");
        Directory.CreateDirectory(_snapshotFolder);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _dbContainer.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        await ExecuteSqlScript("Data/TestData.sql");
    }

    [Test]
    public void ProcessCustomerOrders_WithInvalidCustomerId_ThrowsArgumentException()
    {
        var timeProvider = new FixedTimeProvider(new DateTime(2025, 07, 02, 12, 00, 00));
        var processor = new CustomerOrderProcessor(_connectionString, timeProvider);
        Assert.Throws<ArgumentException>(() => processor.ProcessCustomerOrders(0));
    }

    [Test]
    public void ProcessCustomerOrders_CustomerNotFound_ThrowsException()
    {
        var timeProvider = new FixedTimeProvider(new DateTime(2025, 07, 02, 12, 00, 00));
        var processor = new CustomerOrderProcessor(_connectionString, timeProvider);
        Assert.Throws<Exception>(() => processor.ProcessCustomerOrders(999));
    }

    

    [Test]
    public void Snapshot_OrderWithUnavailableStock()
    {
        RunSnapshotTest(3, "unavailable_stock_order");
    }

    [Test]
    public void Snapshot_RegularCustomerWithSmallOrder()
    {
        RunSnapshotTest(2, "regular_small_order");
    }

    [Test]
    public void Snapshot_VipCustomerWithLargeOrder()
    {
        RunSnapshotTest(1, "vip_large_order");
    }
    
    private object CaptureDatabaseState(int customerId)
    {
        var result = new
        {
            Orders = new List<object>(),
            Inventory = new List<object>(),
            Logs = new List<object>()
        };

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using (var command = new SqlCommand("SELECT Id, CustomerId, Status, TotalAmount, DiscountPercent, DiscountAmount FROM Orders WHERE CustomerId = @CustomerId", connection))
        {
            command.Parameters.AddWithValue("@CustomerId", customerId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Orders.Add(new
                {
                    Id = reader["Id"],
                    CustomerId = reader["CustomerId"],
                    Status = reader["Status"],
                    TotalAmount = reader["TotalAmount"],
                    DiscountPercent = reader["DiscountPercent"],
                    DiscountAmount = reader["DiscountAmount"]
                });
            }
        }

        using (var command = new SqlCommand(@"
        SELECT DISTINCT i.ProductId, i.StockQuantity
        FROM Inventory i
        JOIN OrderItems oi ON oi.ProductId = i.ProductId
        JOIN Orders o ON o.Id = oi.OrderId
        WHERE o.CustomerId = @CustomerId", connection))
        {
            command.Parameters.AddWithValue("@CustomerId", customerId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Inventory.Add(new
                {
                    ProductId = reader["ProductId"],
                    StockQuantity = reader["StockQuantity"]
                });
            }
        }

        using (var command = new SqlCommand(@"
        SELECT ol.OrderId, ol.LogDate, ol.Message
        FROM OrderLogs ol
        JOIN Orders o ON o.Id = ol.OrderId
        WHERE o.CustomerId = @CustomerId", connection))
        {
            command.Parameters.AddWithValue("@CustomerId", customerId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Logs.Add(new
                {
                    OrderId = reader["OrderId"],
                    LogDate = ((DateTime)reader["LogDate"]).ToString("o"), // ISO 8601
                    Message = reader["Message"]
                });
            }
        }

        return result;
    }

    private async Task ExecuteSqlScript(string filePath)
    {
        var script = File.ReadAllText(filePath);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        foreach (var batch in script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
        {
            await using var command = new SqlCommand(batch, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
    
    private void RunSnapshotTest(int customerId, string snapshotName)
    {
        var timeProvider = new FixedTimeProvider(new DateTime(2025, 07, 02, 12, 00, 00));
        var processor = new CustomerOrderProcessor(_connectionString, timeProvider);
        var orders = processor.ProcessCustomerOrders(customerId);

        var outputJson = JsonConvert.SerializeObject(orders, Formatting.Indented);
        var snapshotPath = Path.Combine(_snapshotFolder, $"{snapshotName}_expected.json");

        if (!File.Exists(snapshotPath))
        {
            File.WriteAllText(snapshotPath, outputJson);
            Console.WriteLine($"Created snapshot: {snapshotPath}");
        }
        else
        {
            var expectedJson = File.ReadAllText(snapshotPath);
            Assert.That(outputJson, Is.EqualTo(expectedJson), $"Snapshot mismatch: {snapshotName}");
        }

        var dbState = CaptureDatabaseState(customerId);
        var dbSnapshotPath = Path.Combine(_snapshotFolder, $"{snapshotName}_db_expected.json");
        var dbJson = JsonConvert.SerializeObject(dbState, Formatting.Indented);

        if (!File.Exists(dbSnapshotPath))
        {
            File.WriteAllText(dbSnapshotPath, dbJson);
            Assert.Pass($"Created DB snapshot: {dbSnapshotPath}");
        }
        else
        {
            var expectedDbJson = File.ReadAllText(dbSnapshotPath);
            Assert.That(dbJson, Is.EqualTo(expectedDbJson), $"DB snapshot mismatch: {snapshotName}");
        }
    }

    private static string GetProjectRootPath()
    {
        var targetFolderName = "RefactoringChallenge.Tests.Snapshot";

        var current = new DirectoryInfo(Path.GetDirectoryName(typeof(SnapshotTests).Assembly.Location)!);

        while (current != null && !string.Equals(current.Name, targetFolderName, StringComparison.OrdinalIgnoreCase))
        {
            current = current.Parent;
        }

        if (current != null)
        {
            return Path.Combine(current.FullName, "Data");
        }

        var fallback = Path.Combine(AppContext.BaseDirectory, "Data");
        return fallback;
    }
}