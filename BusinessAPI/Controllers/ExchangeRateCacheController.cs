using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAPI.Data;
using Microsoft.Extensions.Logging;

namespace BusinessAPI.Controllers
{
    [ApiController]
    [Route("api/businessapi")]
    public class ExchangeRateCacheController : ControllerBase
    {
        private readonly ExchangeRateDbContext _context;
        private readonly ILogger<ExchangeRateCacheController> _logger;

        public ExchangeRateCacheController(ExchangeRateDbContext context, ILogger<ExchangeRateCacheController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("exchangeRates")]
        public async Task<IActionResult> GetExchangeRates([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest(new { Message = "Start date must be earlier than end date." });
            }

            _logger.LogInformation($"Querying exchange rates from {startDate} to {endDate}");

            var rates = await _context.ExchangeRates
                .Where(rate => rate.Date >= startDate.ToUniversalTime() && rate.Date <= endDate.ToUniversalTime())
                .OrderBy(rate => rate.Date)
                .ToListAsync();

            _logger.LogInformation($"Found {rates.Count} exchange rates for the specified date range.");

            if (!rates.Any())
            {
                _logger.LogWarning("No exchange rates found for the specified date range.");
                return NotFound(new { Message = "No exchange rates found for the specified date range." });
            }

            return Ok(rates);
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _context.ExchangeRates
                .Select(r => r.Currency)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            if (!currencies.Any())
            {
                return NotFound(new { Message = "No currencies found in the database." });
            }

            return Ok(currencies);
        }
    }
}
