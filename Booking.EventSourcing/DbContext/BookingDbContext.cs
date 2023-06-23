using Booking.EventSourcing.Events;
using Microsoft.EntityFrameworkCore;

namespace Booking.EventSourcing.DbContext;

public class BookingDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public BookingDbContext(DbContextOptions options) : base(options) { }

    public DbSet<BookingEvent> BookingEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(BookingEventConfiguration.Instance);
    }
}
