USE refactoringchallenge;

DELETE FROM OrderLogs;
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM Inventory;
DELETE FROM Products;
DELETE FROM Customers;

DBCC CHECKIDENT ('OrderItems', RESEED, 0);
DBCC CHECKIDENT ('OrderLogs', RESEED, 0);

INSERT INTO Customers (Id, Name, Email, IsVip, RegistrationDate) VALUES 
    (1, 'Joe Doe', 'joe.doe@example.com', 1, '2015-01-01'),     -- VIP, 10 let věrnosti
(2, 'John Smith', 'john@example.com', 0, '2023-03-15'),     -- běžný, mladý účet
(3, 'James Miller', 'miller@example.com', 0, '2024-01-01'); -- běžný, nováček

    INSERT INTO Products (Id, Name, Category, Price) VALUES 
    (1, 'White', 'T-Shirts', 25000),  -- drahý
    (2, 'Gray', 'T-Shirts', 800),     -- levný
    (3, 'Gold', 'T-Shirts', 5000),    -- střední cena
    (4, 'Black', 'T-Shirts', 500);    -- levný

INSERT INTO Inventory (ProductId, StockQuantity) VALUES 
    (1, 100),  -- dost
    (2, 200),  -- dost
    (3, 5),    -- málo
    (4, 50);   -- dost

INSERT INTO Orders (Id, CustomerId, OrderDate, TotalAmount, Status) VALUES 
    (1, 1, '2025-04-10', 0, 'Pending'), -- VIP velká objednávka
(2, 1, '2025-04-12', 0, 'Pending'), -- VIP malá objednávka
(3, 2, '2025-04-13', 0, 'Pending'), -- běžný, malá objednávka
(4, 3, '2025-04-14', 0, 'Pending'); -- objednávka s nedostupným zbožím

    INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES 
    (1, 1, 10, 25000),  -- 250 000
    (1, 2, 5, 800),     -- 4 000 => celkem 254 000
    (2, 4, 3, 500),     -- 1 500
    (3, 2, 1, 800),     -- 800
    (4, 3, 10, 5000);   -- 50 000 ale ve skladu je jen 5 → způsobí OnHold