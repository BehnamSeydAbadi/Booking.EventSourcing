namespace Booking.EventSourcing.Dto;

public record BookingDto
{
    public string SportName { get; set; }
    public DateTime DateTime { get; set; }
    public string UserFullName { get; set; }
    public string UserPhoneNumber { get; set; }
}
