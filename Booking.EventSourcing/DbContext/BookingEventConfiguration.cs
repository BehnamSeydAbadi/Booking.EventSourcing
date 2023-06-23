using Booking.EventSourcing.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.EventSourcing.DbContext;

public class BookingEventConfiguration : IEntityTypeConfiguration<BookingEvent>
{
    public static BookingEventConfiguration Instance => new();

    private BookingEventConfiguration() { }

    public void Configure(EntityTypeBuilder<BookingEvent> builder)
    {
        builder.HasKey(be => be.Id);
        builder.Property(be => be.Type).HasConversion<int>().IsRequired();
        builder.Property(be => be.StreamId).IsRequired();
        builder.Property(be => be.Name).HasMaxLength(256).IsRequired();
        builder.Property(be => be.Data).HasMaxLength(10240).IsRequired();
    }
}
