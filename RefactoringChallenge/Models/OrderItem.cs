namespace RefactoringChallenge.Models;

/// <summary>
/// Represents an item within an order, including details such as quantity, unit price, and associated product.
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Product Product { get; set; }
}
