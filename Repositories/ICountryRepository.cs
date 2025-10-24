namespace BlockedCountriesAPI.Repositories
{
    public interface ICountryRepository
    {
        bool AddBlockedCountry(string code);
        bool RemoveBlockedCountry(string code);
        IEnumerable<string> GetBlockedCountries();
        bool IsCountryBlocked(string code);
        IEnumerable<string> SearchBlockedCountries(string? search);

    }

}
