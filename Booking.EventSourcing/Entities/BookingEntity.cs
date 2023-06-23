namespace Booking.EventSourcing.Entities;

public class BookingEntity
{
    public int Id { get; set; }
    public string SportName { get; set; }
    public DateTime DateTime { get; set; }
    public string UserFullName { get; set; }
    public string UserPhoneNumber { get; set; }
}
