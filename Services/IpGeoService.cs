using Newtonsoft.Json.Linq;

namespace BlockedCountriesAPI.Services
{
    

    public class IpGeoService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public IpGeoService(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        public async Task<(string Code, string Name, string ISP)> GetCountryByIp(string ip)
        {
            var client = _clientFactory.CreateClient();
            var apiKey = _config["IpGeolocation:ApiKey"];

            var url = $"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}&ip={ip}";
            var response = await client.GetStringAsync(url);
            var json = JObject.Parse(response);

            var code = json["country_code2"]?.ToString() ?? "";
            var name = json["country_name"]?.ToString() ?? "";
            var isp = json["isp"]?.ToString() ?? "";

            return (code, name, isp);
        }
    }
}
