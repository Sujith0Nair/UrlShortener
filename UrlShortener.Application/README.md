# UrlShortener.Application

This project is the **Application Layer** in our Clean Architecture. It contains the core business logic and use cases of the application.

## Purpose

The Application Layer orchestrates the flow of data between the API entry points and the lower-level `Domain` and `Infrastructure` layers. It defines *what* the application can do, but not *how* the external concerns (like databases or message brokers) are implemented.

This layer has dependencies on `UrlShortener.Domain` and `UrlShortener.Contracts`, but it has **no knowledge** of the `Infrastructure` layer or the API layers. This separation is a key principle of Clean Architecture.

## Key Components

*   **`IUrlCreationService` / `UrlCreationService`:** The use case for creating a new short URL. It coordinates the `UrlMappingRepository` to save the entity and the `UrlShorteningService` to generate the code.
*   **`IUrlQueryService` / `UrlQueryService`:** The use case for retrieving an original URL. It coordinates with the `ICacheService` and `IUrlMappingRepository` to perform the lookup and with `IMessagePublisher` to send analytics events.
*   **`IUrlShorteningService` / `UrlShorteningService`:** Contains the specific business logic for converting a unique, sequential database ID into a non-predictable, fixed-length (7-character) short code.

    #### Shortening Algorithm Breakdown

    The process ensures that the generated codes are not guessable (e.g., you can't guess the next short code by incrementing the last one) and are of a consistent length.

    **1. ID Shuffling (`ShuffleId` method)**

    To prevent sequential short codes, the unique database `Id` is first "shuffled" into a non-sequential number.

    ```csharp
    private const long Prime = 15485863; // A large prime number
    private const long Salt = 11111111;   // A constant salt value
    private const long Bitmask = (1L << 42) - 1; // Limits the ID to 42 bits

    private static long ShuffleId(long id)
    {
        return ((id * Prime) ^ Salt) & Bitmask;
    }
    ```

    *   `id * Prime`: Multiplies the input `id` by a large prime number. This spreads out the `id` values across a larger range, making them less predictable.
    *   `^ Salt`: Performs a bitwise XOR operation with a constant `Salt`. This further randomizes the bits, ensuring that similar input `id`s don't produce similar intermediate results.
    *   `& Bitmask`: Applies a bitwise AND with a `Bitmask`. This limits the resulting shuffled ID to a specific number of bits (42 in this case), which helps in controlling the length of the final Base-62 encoded string and avoids excessively large numbers.

    **2. Base-62 Encoding (`Encode` method)**

    The shuffled ID is then converted into a Base-62 string using a custom alphabet.

    ```csharp
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private readonly int _base = Alphabet.Length; // 62
    private const int FixedLength = 7;

    public string Encode(long id)
    {
        var shuffleId = ShuffleId(id);

        if (shuffleId == 0) return Alphabet[0].ToString();

        var builder = new StringBuilder();
        while (shuffleId > 0)
        {
            var remainder = (int)(shuffleId % _base);
            builder.Insert(0, Alphabet[remainder]);
            shuffleId /= _base;
        }
        return builder.ToString().PadLeft(FixedLength, Alphabet[0]);
    }
    ```

    *   `Alphabet`: Defines the 62 characters used for encoding (lowercase, uppercase, and digits).
    *   `shuffleId = ShuffleId(id)`: Calls the shuffling function to get the non-sequential ID.
    *   `while (shuffleId > 0)` loop: This is the core Base-62 conversion.
        *   `remainder = (int)(shuffleId % _base)`: Gets the remainder when `shuffleId` is divided by 62. This remainder corresponds to an index in the `Alphabet`.
        *   `builder.Insert(0, Alphabet[remainder])`: Inserts the character from the `Alphabet` at the beginning of the `StringBuilder`. This builds the Base-62 string in reverse order.
        *   `shuffleId /= _base`: Divides `shuffleId` by 62 to prepare for the next iteration.
    *   `PadLeft(FixedLength, Alphabet[0])`: Ensures the final short URL is exactly `FixedLength` (7) characters long by padding with the first character of the `Alphabet` (`a`) if necessary.

*   **`Extensions/ServiceCollectionExtensions.cs`:** Provides extension methods (`AddCommandServices`, `AddQueryServices`) to allow the API projects to register the specific services they need, adhering to the Interface Segregation Principle.
