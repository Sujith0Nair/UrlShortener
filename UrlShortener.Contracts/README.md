# UrlShortener.Contracts

This project is the **Contracts Layer** of the application.

## Purpose

This project contains shared data contracts, such as Data Transfer Objects (DTOs), event models, and service interfaces. It defines the "language" of communication between the different layers of the application.

This layer can depend on `UrlShortener.Domain` but should not have dependencies on `Application` or `Infrastructure`.
