namespace DataAPI.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string? Currency { get; set; }
        public decimal Rate { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
