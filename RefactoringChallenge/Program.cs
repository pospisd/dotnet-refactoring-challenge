using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Abstractions.Services;
using RefactoringChallenge.Abstractions.Services.Discounts;
using RefactoringChallenge.Configuration;
using RefactoringChallenge.Data;
using RefactoringChallenge.Data.Repositories;
using RefactoringChallenge.Services;
using RefactoringChallenge.Services.Discounts;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true);

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Db"));

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IUnitOfWorkFactory, SqlUnitOfWorkFactory>();
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

builder.Services.AddScoped<IDiscountRule, VipDiscountRule>();
builder.Services.AddScoped<IDiscountRule, LoyaltyDiscountRule>();
builder.Services.AddScoped<IDiscountRule, OrderAmountDiscountRule>();
builder.Services.AddScoped<IDiscountCalculator, DiscountCalculator>();

builder.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();
builder.Services.AddScoped<ICustomerOrderProcessor, CustomerOrderProcessor>();


var host = builder.Build();
host.Run();
