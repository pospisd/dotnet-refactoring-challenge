# refactoring-challenge

## Tasks

1. **Refactor the Codebase**  
   - Apply appropriate design patterns such as **Repository** and **Service** to improve modularity, testability, and maintainability.  
   - Separate the data access layer from the business logic using abstractions.  

2. **Implement Dependency Injection**  
   - Integrate a Dependency Injection container to inject required dependencies.  
   - Ensure the solution adheres to the Inversion of Control principle.  

3. **Write Unit Tests**  
   - Add unit tests for the business logic using mocks or stubs where appropriate.  

## Analysis

- The project is a single `.NET 9` SDK Worker project (`Microsoft.NET.Sdk.Worker`).
- It uses Microsoft SQL Server (via Docker) as the database backend.
- Data access is implemented using raw ADO.NET (no ORM).
- NUnit is used for unit testing.
- The entire codebase (business logic, tests, schema) is located in a single project.  
- All business logic is concentrated in one method: `CustomerOrderProcessor.ProcessCustomerOrders`.  
- Tests connect to a live SQL Server instance (no mocking or abstraction used).  
- The application does not provide any executable functionality (no background services, no endpoints).  
- No approach to schema versioning or database migration is in place (by design – see Out of Scope).  
- Configuration is not used (`IConfiguration` is not implemented); connection strings and credentials are hardcoded.  
- While `UserSecretsId` is defined in the project file, the user secrets mechanism is not utilized.  
- Compiler warnings are present and not yet resolved.  

## Out of Scope

- Creating and exposing any endpoints or background services.  
- Implementing a database schema migration or versioning mechanism.  
- Infrastructure topics such as CI/CD pipelines, deployment, or secret management.  
- Updating or producing user-facing documentation (e.g., README, API documentation).  

## Ideas / Extras (Optional)

- Most of the out-of-scope items could be considered for future iterations.  
- Move hardcoded strings (SQL, statuses like `"Processed"`, `"OnHold"`) to constants or enums.  
- Clean Git history to remove accidentally committed secrets (e.g., DB password).  
- Add integration tests using a containerized test database (e.g., via Docker).  

## Progress Log

- Performed initial analysis and documentation. Defined goals, scope, and out-of-scope boundaries.  ~30min
- Implemented snapshot-based tests for order processing to verify functional consistency. ~80min
- Extracted responsibilities from `CustomerOrderProcessor` into dedicated methods ~20min


## Notes

- introduce serilog for logging
