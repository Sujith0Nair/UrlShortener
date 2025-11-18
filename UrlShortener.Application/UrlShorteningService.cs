using System.Text;
using UrlShortener.Contracts;

namespace UrlShortener.Application;

public class UrlShorteningService : IUrlShorteningService
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private readonly int _base = Alphabet.Length;

    private const int FixedLength = 7;
    private const long Prime = 15485863;
    private const long Salt = 11111111;
    private const long Bitmask = (1L << 42) - 1;
    
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
    
    private static long ShuffleId(long id)
    {
        return ((id * Prime) ^ Salt) & Bitmask;
    }
}