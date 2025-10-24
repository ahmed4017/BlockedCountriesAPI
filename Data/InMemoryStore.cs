using System.Collections.Concurrent;
using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Data
{
    public class InMemoryStore
    {
        public ConcurrentDictionary<string, bool> BlockedCountries { get; set; } = new();

        public List<BlockedAttemptLog> BlockedAttempts { get; set; } = new();
    }
}
