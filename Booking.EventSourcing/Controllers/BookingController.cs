using Booking.EventSourcing.Dto;
using Booking.EventSourcing.Entities;
using Booking.EventSourcing.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Booking.EventSourcing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly BookingRepository _repository;

    public BookingController(BookingRepository repository) => _repository = repository;

    [HttpPost]
    public async Task<IActionResult> Post(BookingDto dto)
    {
        var id = await _repository.CreateAsync(new BookingEntity
        {
            SportName = dto.SportName,
            UserFullName = dto.UserFullName,
            UserPhoneNumber = dto.UserPhoneNumber,
            DateTime = dto.DateTime,
        });
        return Ok(id);
    }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var bookings = await _repository.GetAsync();
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var entity = await _repository.GetAsync(id);
        return Ok(entity);
    }

    [HttpGet("History/{id}")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var streams = await _repository.GetStreamAsync(id);
        return Ok(streams);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, BookingDto dto)
    {
        await _repository.UpdateAsync(id, new BookingEntity
        {
            Id = id,
            SportName = dto.SportName,
            UserFullName = dto.UserFullName,
            UserPhoneNumber = dto.UserPhoneNumber,
            DateTime = dto.DateTime,
        });

        return Ok();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return Ok();
    }
}
