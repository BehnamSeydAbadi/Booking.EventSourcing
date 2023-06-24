using Booking.EventSourcing.DbContext;
using Booking.EventSourcing.Entities;
using Booking.EventSourcing.Events;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Booking.EventSourcing.Repositories;

public class BookingRepository
{
    private readonly BookingDbContext _dbContext;

    public BookingRepository(BookingDbContext dbContext) => _dbContext = dbContext;

    public async Task<int> CreateAsync(BookingEntity entity)
    {
        var eventEntity = new BookingEvent
        {
            Data = JsonConvert.SerializeObject(entity),
            Type = EventType.Create,
            Name = "Created",
            DateTime = DateTime.Now,
        };

        _dbContext.BookingEvents.Add(eventEntity);

        await _dbContext.SaveChangesAsync();

        return eventEntity.StreamId;
    }


    public async Task<BookingEntity?> GetAsync(int streamId)
    {
        var events = await _dbContext.BookingEvents
            .Where(be => be.StreamId == streamId)
            .OrderBy(be => be.DateTime).ToArrayAsync();

        var entity = new BookingEntity();

        foreach (var @event in events)
        {
            var data = JsonConvert.DeserializeObject<BookingEntity>(@event.Data)!;

            switch (@event.Type)
            {
                case EventType.Create:
                    {
                        entity.Id = data.Id;
                        entity.SportName = data.SportName;
                        entity.DateTime = data.DateTime;
                        entity.UserFullName = data.UserFullName;
                        entity.UserPhoneNumber = data.UserPhoneNumber;
                    }
                    break;
                case EventType.Update:
                    {
                        entity.SportName = data.SportName;
                        entity.DateTime = data.DateTime;
                        entity.UserFullName = data.UserFullName;
                        entity.UserPhoneNumber = data.UserPhoneNumber;
                    }
                    break;
                case EventType.Delete:
                    return default;
            }
        }

        return entity;
    }
    public async Task<IEnumerable<BookingEvent>> GetStreamAsync(int streamId)
    {
        return await _dbContext.BookingEvents
             .Where(be => be.StreamId == streamId)
             .OrderBy(be => be.DateTime).ToArrayAsync();
    }


    public async Task UpdateAsync(int streamId, BookingEntity entity)
    {
        if (await _dbContext.BookingEvents.AnyAsync(be => be.StreamId == streamId) is false)
            throw new InvalidDataException(streamId.ToString());

        var eventEntity = new BookingEvent
        {
            StreamId = streamId,
            Data = JsonConvert.SerializeObject(entity),
            Type = EventType.Update,
            Name = "Updated",
            DateTime = DateTime.Now,
        };

        _dbContext.BookingEvents.Add(eventEntity);

        await _dbContext.SaveChangesAsync();
    }


    public async Task DeleteAsync(int streamId)
    {
        if (await _dbContext.BookingEvents.AnyAsync(be => be.StreamId == streamId) is false)
            throw new InvalidDataException(streamId.ToString());

        var eventEntity = new BookingEvent
        {
            StreamId = streamId,
            Type = EventType.Delete,
            Name = "Deleted",
            DateTime = DateTime.Now,
        };

        _dbContext.BookingEvents.Add(eventEntity);

        await _dbContext.SaveChangesAsync();
    }
}