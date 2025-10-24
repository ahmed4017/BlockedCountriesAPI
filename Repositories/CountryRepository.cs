using System;
using System.Linq;
using BlockedCountriesAPI.Data;
using System.Collections.Concurrent;

namespace BlockedCountriesAPI.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly InMemoryStore _store;

        public CountryRepository(InMemoryStore store)
        {
            _store = store;
        }

        public bool AddBlockedCountry(string code)
        {
            code = code.ToUpper();
            return _store.BlockedCountries.TryAdd(code, true);
        }

        public bool RemoveBlockedCountry(string code)
        {
            code = code.ToUpper();
            return _store.BlockedCountries.TryRemove(code, out _);
        }

        public IEnumerable<string> GetBlockedCountries()
        {
            return _store.BlockedCountries.Keys.ToList();
        }

        public bool IsCountryBlocked(string code)
        {
            return _store.BlockedCountries.ContainsKey(code.ToUpper());
        }

        public IEnumerable<string> SearchBlockedCountries(string? search)
        {
            var countries = _store.BlockedCountries.Keys.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
                countries = countries.Where(c => c.Contains(search, StringComparison.OrdinalIgnoreCase));

            return countries;
        }

    }
}
