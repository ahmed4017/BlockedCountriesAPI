using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _repository;

        public CountriesController(ICountryRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest("Country code is required.");

            var added = _repository.AddBlockedCountry(countryCode);
            if (!added)
                return Conflict($"{countryCode.ToUpper()} already blocked.");

            return Ok($"{countryCode.ToUpper()} blocked successfully.");
        }



        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest("Country code is required.");

            var removed = _repository.RemoveBlockedCountry(countryCode);
            if (!removed)
                return NotFound($"{countryCode.ToUpper()} is not in the blocked list.");

            return Ok($"{countryCode.ToUpper()} unblocked successfully.");
        }


        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("page and pageSize must be greater than 0.");

            var all = _repository.SearchBlockedCountries(search).ToList();
            var total = all.Count;

            var items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<string>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(result);
        }


    }
}
