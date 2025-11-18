# UrlShortener.Infrastructure

This project is the **Infrastructure Layer** in our Clean Architecture. It contains the concrete implementations for all external concerns, such as databases, caches, and message brokers.

## Purpose

The Infrastructure Layer is responsible for the "how" of the application. It provides the concrete tools and technologies that fulfill the contracts (interfaces) defined in the `Application` and `Domain` layers. This layer is the most volatile part of the application, as technologies can be swapped out (e.g., changing from PostgreSQL to SQL Server, or from RabbitMQ to Azure Service Bus).

Because this layer implements interfaces from the `Application` layer, the dependency is "inverted," which is a core tenet of Clean Architecture.

## Key Components

*   **`ApplicationDbContext`:** The Entity Framework Core `DbContext` for interacting with the PostgreSQL database.
*   **`UrlMappingRepository`:** The concrete implementation of `IUrlMappingRepository`, using EF Core to perform database operations.
*   **`RedisCacheService`:** The concrete implementation of `ICacheService`, using the `StackExchange.Redis` library.
*   **`RabbitMqPublisher`:** The concrete implementation of `IMessagePublisher`, using the `RabbitMQ.Client` library to send messages.
*   **`LinkVisitedEventConsumer`:** An `IHostedService` (background service) that listens for `LinkVisitedEvent` messages from RabbitMQ and updates the database.
*   **`Extensions/ServiceCollectionExtensions.cs`:** Provides a single `AddInfrastructureServices` extension method to encapsulate the registration of all infrastructure-related services.
