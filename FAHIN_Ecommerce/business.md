# FAHIN Ecommerce Business Logic & Architecture

## Overview
FAHIN Ecommerce is a high-performance, scalable monolith application built on .NET 8/10 and Razor Views. It follows a decoupled architecture where the database context is kept minimal, and all business logic resides in a dedicated service layer.

## Architectural Strategy: Clean Separation of Concerns
To ensure the system remains robust while targeting millions of users, we have implemented a strict separation between data access and business logic:
- **Minimal dbContext**: The `dbContext` class is strictly limited to constructors and `DbSet` declarations. It contains no methods, functions, or model configuration logic.
- **Service Layer**: All data operations—including soft-delete filtering (`isDelete == 0`), audit field population (`createdAt`, `createdBy`), and transactional integrity—are handled within specialized services (`ProductService`, `OrderService`, etc.).
- **Controller-Service Interaction**: Controllers never interact with the `dbContext` directly. They rely on the service layer to perform all data-related tasks. This ensures that business rules are applied consistently across the entire application.

## Key Scalability Features
- **Manual Filtering**: By handling soft-delete filters in the service layer, we maintain full control over query performance and prevent unnecessary overhead in the ORM.
- **Async Execution**: Every database operation is executed asynchronously to maximize the throughput of the server.
- **Audit Consistency**: Audit trails are managed centrally within the service layer, ensuring that every record's lineage is accurately preserved.
- **One-Way Data Binding**: Entities remain lightweight and independent, avoiding the pitfalls of deep object graphs and circular dependencies.

## Security & Access
- **Identity Integration**: Fully integrated with ASP.NET Core Identity for secure user and role management.
- **Role-Based UI**: Dynamic layout switching ensures that Admins and General Users receive an interface tailored to their specific roles and permissions.
- **JWT Authentication**: Prepared for multi-platform support with token-based authentication services.
