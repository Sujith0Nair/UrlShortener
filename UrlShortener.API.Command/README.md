# UrlShortener.API.Command

This project is the **Command API** for the URL Shortener application. It represents the "Write" side of the CQRS (Command Query Responsibility Segregation) pattern.

## Purpose

The primary responsibility of this API is to handle all state-changing operations, specifically the creation of new short URLs. It is designed to be secure and robust, prioritizing data integrity.

## Key Responsibilities

*   **Endpoint:** Exposes the `POST /shorten` endpoint to accept new long URLs.
*   **Authentication:** Secures the endpoint using custom `ApiKeyAuthenticationMiddleware`. It validates an `X-Api-Key` provided in the request header against a configured list of keys.
*   **Rate Limiting:** Applies a dynamic, tier-based rate limiting policy. Based on the validated API key, it enforces different usage limits (e.g., "free-tier" vs. "pro-tier").
*   **Orchestration:** Acts as the Composition Root for the command path, wiring up all necessary services from the `Application` and `Infrastructure` layers.

## Configuration

This project requires the following configuration to be present in `appsettings.json` and `appsettings.Development.json`:

*   `ConnectionStrings` for PostgreSQL, Redis, and RabbitMQ.

Additionally, it requires User Secrets to be configured for storing API keys. See the main `README.md` for setup instructions.
