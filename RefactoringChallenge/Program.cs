using RefactoringChallenge.Abstractions.Data;
using RefactoringChallenge.Abstractions.Data.Repositories;
using RefactoringChallenge.Configuration;
using RefactoringChallenge.Data;
using RefactoringChallenge.Data.Repositories;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true);

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Db"));
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IUnitOfWorkFactory, SqlUnitOfWorkFactory>();
builder.Services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();

var host = builder.Build();
host.Run();
