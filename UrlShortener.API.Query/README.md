# UrlShortener.API.Query

This project is the **Query API** for the URL Shortener application. It represents the "Read" side of the CQRS (Command Query Responsibility Segregation) pattern and is highly optimized for performance.

## Purpose

The sole responsibility of this API is to handle URL redirection. It is designed to be extremely fast and scalable to handle a high volume of read requests, as this is the most common operation in a URL shortener.

## Key Responsibilities

*   **Endpoint:** Exposes the `GET /{shortUrl}` endpoint to handle incoming redirect requests.
*   **High-Performance Lookups:** Implements a **cache-aside pattern**. It first attempts to find the original URL in a Redis cache. If a cache miss occurs, it falls back to the PostgreSQL database and then populates the cache for subsequent requests.
*   **Asynchronous Analytics:** Upon a successful URL lookup, this service publishes a `LinkVisitedEvent` message to a RabbitMQ exchange. It does **not** wait for the message to be processed. This decouples the non-critical analytics work from the user's critical path, ensuring the redirect is not delayed.
*   **Orchestration:** Acts as the Composition Root for the query path, wiring up all necessary services from the `Application` and `Infrastructure` layers.

## Configuration

This project requires the following configuration to be present in `appsettings.json` and `appsettings.Development.json`:

*   `ConnectionStrings` for PostgreSQL, Redis, and RabbitMQ.
