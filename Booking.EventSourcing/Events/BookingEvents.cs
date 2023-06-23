namespace Booking.EventSourcing.Events;

public class BookingEvents
{
    public int Id { get; set; }

    public int StreamId { get; set; }
    public int Version { get; set; }
    public EventType Type { get; set; }
    public string Name { get; set; }
    public string Data { get; set; }
}