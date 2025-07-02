using Microsoft.Data.SqlClient;

namespace RefactoringChallenge;

public class CustomerOrderProcessor
{
    private readonly ITimeProvider _timeProvider = new SystemTimeProvider();
    private readonly string _connectionString = "Server=localhost,1433;Database=refactoringchallenge;User ID=sa;Password=RCPassword1!;";

    public CustomerOrderProcessor() { }

    internal CustomerOrderProcessor(string connectionString, ITimeProvider timeProvider)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null.");
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider), "Time provider cannot be null.");
    }

    /// <summary></summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public List<Order> ProcessCustomerOrders(int customerId)
    {
        if (customerId <= 0)
            throw new ArgumentException("ID zákazníka musí být kladné číslo.", nameof(customerId));

        var processedOrders = new List<Order>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var customer = GetCustomerById(connection, customerId);
            var pendingOrders = GetPendingOrders(connection, customerId);

            foreach (var order in pendingOrders)
            {
                LoadOrderItems(connection, order);
                LoadProductsForItems(connection, order.Items);
            }

            foreach (var order in pendingOrders)
            {
                ProcessOrder(connection, customer, order);
                processedOrders.Add(order);
            }
        }

        return processedOrders;
    }

    // Zpracuje jednu objednávku – vypočítá slevu, aktualizuje databázi, upraví stav zásob a zaloguje.
    private void ProcessOrder(SqlConnection connection, Customer customer, Order order)
    {
        CalculateDiscount(customer, order);
        UpdateOrderInDatabase(connection, order);

        if (AreAllItemsInStock(connection, order))
        {
            ReduceInventory(connection, order);
            order.Status = "Ready";
            UpdateOrderStatus(connection, order);
            InsertOrderLog(connection, order.Id, $"Order completed with {order.DiscountPercent}% discount. Total price: {order.TotalAmount}");
        }
        else
        {
            order.Status = "OnHold";
            UpdateOrderStatus(connection, order);
            InsertOrderLog(connection, order.Id, "Order on hold. Some items are not on stock.");
        }
    }

    // Načte zákazníka z databáze podle ID.
    // TODO: Vydělit do CustomerRepository.
    private Customer GetCustomerById(SqlConnection connection, int customerId)
    {
        using var command = new SqlCommand("SELECT Id, Name, Email, IsVip, RegistrationDate FROM Customers WHERE Id = @CustomerId", connection);
        command.Parameters.AddWithValue("@CustomerId", customerId);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Customer
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                IsVip = (bool)reader["IsVip"],
                RegistrationDate = (DateTime)reader["RegistrationDate"]
            };
        }

        throw new Exception($"Zákazník s ID {customerId} nebyl nalezen.");
    }

    // Načte všechny čekající objednávky daného zákazníka.
    // TODO: Přesunout do OrderRepository.
    private List<Order> GetPendingOrders(SqlConnection connection, int customerId)
    {
        var orders = new List<Order>();
        using var command = new SqlCommand("SELECT Id, CustomerId, OrderDate, TotalAmount, Status FROM Orders WHERE CustomerId = @CustomerId AND Status = 'Pending'", connection);
        command.Parameters.AddWithValue("@CustomerId", customerId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            orders.Add(new Order
            {
                Id = (int)reader["Id"],
                CustomerId = (int)reader["CustomerId"],
                OrderDate = (DateTime)reader["OrderDate"],
                TotalAmount = (decimal)reader["TotalAmount"],
                Status = (string)reader["Status"]
            });
        }

        return orders;
    }

    // Načte položky objednávky.
    // TODO: Přesunout do OrderRepository nebo OrderItemRepository.
    private void LoadOrderItems(SqlConnection connection, Order order)
    {
        order.Items = new List<OrderItem>();

        using var command = new SqlCommand("SELECT Id, OrderId, ProductId, Quantity, UnitPrice FROM OrderItems WHERE OrderId = @OrderId", connection);
        command.Parameters.AddWithValue("@OrderId", order.Id);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            order.Items.Add(new OrderItem
            {
                Id = (int)reader["Id"],
                OrderId = (int)reader["OrderId"],
                ProductId = (int)reader["ProductId"],
                Quantity = (int)reader["Quantity"],
                UnitPrice = (decimal)reader["UnitPrice"]
            });
        }
    }

    // Načte produkty přiřazené ke každé položce objednávky.
    // TODO: Přesunout do ProductRepository.
    private void LoadProductsForItems(SqlConnection connection, List<OrderItem> items)
    {
        foreach (var item in items)
        {
            using var command = new SqlCommand("SELECT Id, Name, Category, Price FROM Products WHERE Id = @ProductId", connection);
            command.Parameters.AddWithValue("@ProductId", item.ProductId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                item.Product = new Product
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Category = (string)reader["Category"],
                    Price = (decimal)reader["Price"]
                };
            }
        }
    }

    // Spočítá slevu pro danou objednávku na základě zákazníka a hodnoty nákupu.
    // TODO: Přesunout, asi něco jako DiscountCalculator.
    private void CalculateDiscount(Customer customer, Order order)
    {
        decimal totalAmount = 0;
        foreach (var item in order.Items)
        {
            totalAmount += item.Quantity * item.UnitPrice;
        }

        decimal discountPercent = 0;

        if (customer.IsVip) discountPercent += 10;

        int years = _timeProvider.Now.Year - customer.RegistrationDate.Year;
        if (years >= 5) discountPercent += 5;
        else if (years >= 2) discountPercent += 2;

        if (totalAmount > 10000) discountPercent += 15;
        else if (totalAmount > 5000) discountPercent += 10;
        else if (totalAmount > 1000) discountPercent += 5;

        if (discountPercent > 25) discountPercent = 25;

        var discountAmount = totalAmount * (discountPercent / 100);
        var finalAmount = totalAmount - discountAmount;

        order.DiscountPercent = discountPercent;
        order.DiscountAmount = discountAmount;
        order.TotalAmount = finalAmount;
        order.Status = "Processed";
    }

    // Aktualizuje objednávku v databázi s novými hodnotami.
    // TODO: Přesunout do OrderRepository.
    private void UpdateOrderInDatabase(SqlConnection connection, Order order)
    {
        using var command = new SqlCommand("UPDATE Orders SET TotalAmount = @TotalAmount, DiscountPercent = @DiscountPercent, DiscountAmount = @DiscountAmount, Status = @Status WHERE Id = @OrderId", connection);
        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
        command.Parameters.AddWithValue("@DiscountPercent", order.DiscountPercent);
        command.Parameters.AddWithValue("@DiscountAmount", order.DiscountAmount);
        command.Parameters.AddWithValue("@Status", order.Status);
        command.Parameters.AddWithValue("@OrderId", order.Id);

        command.ExecuteNonQuery();
    }

    // Zkontroluje, zda jsou všechny položky objednávky skladem.
    // TODO: Přesunout do InventoryService.
    private bool AreAllItemsInStock(SqlConnection connection, Order order)
    {
        foreach (var item in order.Items)
        {
            using var command = new SqlCommand("SELECT StockQuantity FROM Inventory WHERE ProductId = @ProductId", connection);
            command.Parameters.AddWithValue("@ProductId", item.ProductId);

            var stock = (int?)command.ExecuteScalar();
            if (stock == null || stock < item.Quantity)
            {
                return false;
            }
        }

        return true;
    }

    // Sníží zásoby podle položek v objednávce.
    // TODO: Přesunout do InventoryService.
    private void ReduceInventory(SqlConnection connection, Order order)
    {
        foreach (var item in order.Items)
        {
            using var command = new SqlCommand("UPDATE Inventory SET StockQuantity = StockQuantity - @Quantity WHERE ProductId = @ProductId", connection);
            command.Parameters.AddWithValue("@Quantity", item.Quantity);
            command.Parameters.AddWithValue("@ProductId", item.ProductId);

            command.ExecuteNonQuery();
        }
    }

    // Aktualizuje stav objednávky v databázi.
    // TODO: Přesunout do OrderRepository.
    private void UpdateOrderStatus(SqlConnection connection, Order order)
    {
        using var command = new SqlCommand("UPDATE Orders SET Status = @Status WHERE Id = @OrderId", connection);
        command.Parameters.AddWithValue("@Status", order.Status);
        command.Parameters.AddWithValue("@OrderId", order.Id);
        command.ExecuteNonQuery();
    }

    // Vloží záznam do logu objednávky.
    // TODO: Přesunout možná do OrderService/OrderRepository
    private void InsertOrderLog(SqlConnection connection, int orderId, string message)
    {
        using var command = new SqlCommand("INSERT INTO OrderLogs (OrderId, LogDate, Message) VALUES (@OrderId, @LogDate, @Message)", connection);
        command.Parameters.AddWithValue("@OrderId", orderId);
        command.Parameters.AddWithValue("@LogDate", _timeProvider.Now);
        command.Parameters.AddWithValue("@Message", message);
        command.ExecuteNonQuery();
    }
}

