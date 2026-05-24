# FAHIN Ecommerce Business Logic & Architecture

## Overview
FAHIN Ecommerce is a high-performance, scalable monolith application built on .NET 8/10 and Razor Views. It is designed to handle millions of users with a strict emphasis on performance and data integrity.

## Architectural Strategy: One-Way Data Binding
To target millions of users without crashing the system, we have implemented **One-Way Data Binding** in our domain models. 
- **Explicit Navigation**: We have removed `ICollection<T>` navigation properties from all entities. 
- **Performance Benefits**: This prevents EF Core from accidentally performing large, implicit joins or "lazy loading" massive collections of data (e.g., loading thousands of products when querying a category).
- **Control**: All relationship data must be queried explicitly. This ensures that developers are always aware of the data volume being retrieved, leading to more optimized SQL queries and lower memory consumption.

## Core Components

### 1. Identity & Access Management
- **ApplicationUser**: Extended Identity user with profile management.
- **ApplicationRole**: Custom roles for Admin and Customer access.
- **JWT + Cookie Auth**: Secure multi-channel authentication as per `Info.md`.

### 2. Domain Entities (`Data\Entity`)
- **BaseEntity**: Unified structure with `camelCase` audit fields (`isDelete`, `createdAt`, etc.).
- **Category**: Hierarchical organization using `parentCategoryId`.
- **Product**: Indexed catalog items optimized for high-speed lookup.
- **Order & OrderItem**: Transactional records with fixed unit prices at purchase time.
- **PurchaseLog**: High-fidelity audit trail for all sales transactions.

### 3. Data Access (`Context\dbContext.cs`)
- **IdentityDbContext**: Integrated with custom identity entities.
- **High Timeout**: Configured for large-scale data operations as specified in `Info.md`.
- **Global Query Filters**: Automated handling of soft deletes across the entire platform.
- **Auditing**: Automatic population of audit fields using `IHttpContextAccessor`.

## Scaling for Millions
- **SQL Indexing**: Strategic indexes on `name`, `userId`, `categoryId`, and `orderDate`.
- **Asynchronous Flow**: 100% async database operations to maximize thread pool efficiency.
- **Monolith Efficiency**: Server-side rendering with Razor Views reduces client-side processing overhead and improves SEO.
