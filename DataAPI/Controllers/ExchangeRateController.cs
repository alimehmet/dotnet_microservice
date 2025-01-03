using Microsoft.AspNetCore.Mvc;
using DataAPI.Data;          
using DataAPI.Models;          
using System.Xml.Linq;         
using Microsoft.Extensions.Logging;

namespace DataAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ExchangeRateDbContext _context;
        private readonly ILogger<ExchangeRateController> _logger;

        public ExchangeRateController(ExchangeRateDbContext context, ILogger<ExchangeRateController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("sync")] 
        public async Task<IActionResult> SyncExchangeRates()
        {
            var url = "https://www.tcmb.gov.tr/kurlar/today.xml";

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; ExchangeRateApp/1.0)");

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var xmlDocument = XDocument.Parse(xmlContent);

                    var exchangeRates = xmlDocument.Descendants("Currency")
                        .Where(x => x.Element("ForexBuying") != null && x.Element("CurrencyName") != null)
                        .Select(x => new ExchangeRate
                        {
                            Currency = x.Element("CurrencyName")?.Value,
                            Rate = decimal.TryParse(x.Element("ForexBuying")?.Value, out var rate) ? rate : 0,
                            Date = DateTimeOffset.Parse(xmlDocument.Root.Attribute("Tarih").Value).ToUniversalTime() 
                        }).ToList();

                    _logger.LogInformation($"Parsed {exchangeRates.Count} exchange rates from XML.");

                    foreach (var rate in exchangeRates)
                    {
                        if (!_context.ExchangeRates.Any(r => r.Currency == rate.Currency && r.Date == rate.Date))
                        {
                            _context.ExchangeRates.Add(rate);
                            _logger.LogInformation($"Added exchange rate: {rate.Currency} - {rate.Rate} - {rate.Date}");
                        }
                    }

                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Exchange rates synced successfully", Count = exchangeRates.Count });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, 
                        new { Message = $"Failed to fetch data. Please try again. Reason: {response.ReasonPhrase}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while syncing exchange rates.");
                return StatusCode(500, new { Message = "An error occurred while syncing exchange rates.", Error = ex.Message });
            }
        }

        [HttpGet("sync-range")]
        public async Task<IActionResult> SyncExchangeRatesRange()
        {
            var startDate = DateTimeOffset.Now.AddMonths(-2).Date;
            var endDate = DateTimeOffset.Now.Date;

            int totalRates = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var url = $"https://www.tcmb.gov.tr/kurlar/{date:yyyyMM}/{date:ddMMyyyy}.xml";
                try
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var xmlContent = await response.Content.ReadAsStringAsync();
                        var xmlDocument = XDocument.Parse(xmlContent);

                        var exchangeRates = xmlDocument.Descendants("Currency")
                            .Where(x => x.Element("ForexBuying") != null && x.Element("CurrencyName") != null)
                            .Select(x => new ExchangeRate
                            {
                                Currency = x.Element("CurrencyName")?.Value,
                                Rate = decimal.TryParse(x.Element("ForexBuying")?.Value, out var rate) ? rate : 0,
                                Date = date.ToUniversalTime() 
                            }).ToList();

                        _logger.LogInformation($"Parsed {exchangeRates.Count} exchange rates from XML for date {date:yyyy-MM-dd}.");

                        foreach (var rate in exchangeRates)
                        {
                            if (!_context.ExchangeRates.Any(r => r.Currency == rate.Currency && r.Date == rate.Date))
                            {
                                _context.ExchangeRates.Add(rate);
                                _logger.LogInformation($"Added exchange rate: {rate.Currency} - {rate.Rate} - {rate.Date}");
                                totalRates++;
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore errors for missing dates
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Fetched rates for last two months.", Total = totalRates });
        }
    }
}
