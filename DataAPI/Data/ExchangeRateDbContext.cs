using Microsoft.EntityFrameworkCore;
using DataAPI.Models;

namespace DataAPI.Data
{
    public class ExchangeRateDbContext : DbContext
    {
        public ExchangeRateDbContext(DbContextOptions<ExchangeRateDbContext> options)
            : base(options)
        {
        }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.ToTable("ExchangeRates");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Currency)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Rate)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Date)
                      .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
