﻿namespace Booking.EventSourcing.Events;

public class BookingEvent
{
    public int Id { get; set; }
    public int StreamId { get; set; }
    public string Name { get; set; }
    public EventType Type { get; set; }
    public string Data { get; set; }
    public DateTime DateTime { get; set; }
}