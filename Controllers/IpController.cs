using BlockedCountriesAPI.Data;
using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/ip")]
public class IpController : ControllerBase
{
    private readonly IpGeoService _ipGeoService;
    private readonly ICountryRepository _countryRepository;
    private readonly InMemoryStore _store;

    public IpController(IpGeoService ipGeoService, ICountryRepository countryRepository, InMemoryStore store)
    {
        _ipGeoService = ipGeoService;
        _countryRepository = countryRepository;
        _store = store;
    }



    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
    {
        try
        {
            ipAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrWhiteSpace(ipAddress))
                return BadRequest("IP address is required or could not be detected.");

            if (!IsValidIp(ipAddress))
                return BadRequest("Invalid IP address format.");

            var (code, name, isp) = await _ipGeoService.GetCountryByIp(ipAddress);

            if (string.IsNullOrEmpty(code))
                return NotFound("Could not retrieve country information for this IP.");

            return Ok(new
            {
                IpAddress = ipAddress,
                CountryCode = code,
                CountryName = name,
                ISP = isp
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error while fetching IP info: {ex.Message}");
        }
    }

    private bool IsValidIp(string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }


    [HttpGet("check-block")]
    public async Task<IActionResult> CheckBlock()
    {
        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            //if (ip == "::1" || ip == "127.0.0.1")
            //    ip = "8.8.8.8"; // IP خارجي معروف


            if (string.IsNullOrWhiteSpace(ip))
                return BadRequest("Unable to detect IP address.");

            var (code, name, isp) = await _ipGeoService.GetCountryByIp(ip);

            if (string.IsNullOrEmpty(code))
                return StatusCode(500, "Could not determine country from IP.");

            bool isBlocked = _countryRepository.IsCountryBlocked(code);

            _store.BlockedAttempts.Add(new BlockedAttemptLog
            {
                IpAddress = ip,
                CountryCode = code,
                IsBlocked = isBlocked,
                Timestamp = DateTime.UtcNow,
                UserAgent = Request.Headers["User-Agent"],
                Isp = isp
            });

            return Ok(new
            {
                IpAddress = ip,
                CountryCode = code,
                CountryName = name,
                IsBlocked = isBlocked,
                CheckedAt = DateTime.UtcNow

            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error checking block: {ex.Message}");
        }
    }


}
