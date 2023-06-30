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
        var streamId = (await GetLastStreamIdAsync()) + 1;

        entity.Id = streamId;

        var eventEntity = new BookingEvent
        {
            Data = JsonConvert.SerializeObject(entity),
            Type = EventType.Create,
            Name = "Created",
            DateTime = DateTime.Now,
            StreamId = streamId
        };

        _dbContext.BookingEvents.Add(eventEntity);

        await _dbContext.SaveChangesAsync();

        return eventEntity.StreamId;
    }

    public async Task<BookingEntity?> GetAsync(int id)
    {
        var events = await _dbContext.BookingEvents
            .Where(be => be.StreamId == id)
            .OrderBy(be => be.DateTime).ToArrayAsync();

        return Mutate(events);
    }
    public async Task<IEnumerable<BookingEntity>> GetAsync()
    {
        var streams = (await _dbContext.BookingEvents.ToArrayAsync()).GroupBy(be => be.StreamId).ToArray();

        var entities = new List<BookingEntity>();

        foreach (var events in streams)
            entities.Add(Mutate(events));

        return entities;
    }
    public async Task<IEnumerable<BookingEvent>> GetStreamAsync(int id)
    {
        return await _dbContext.BookingEvents
             .Where(be => be.StreamId == id)
             .OrderBy(be => be.DateTime).ToArrayAsync();
    }


    public async Task UpdateAsync(int id, BookingEntity entity)
    {
        if (await AnyAsync(id) is false)
            throw new InvalidDataException(id.ToString());

        var eventEntity = new BookingEvent
        {
            StreamId = id,
            Data = JsonConvert.SerializeObject(entity),
            Type = EventType.Update,
            Name = "Updated",
            DateTime = DateTime.Now,
        };

        _dbContext.BookingEvents.Add(eventEntity);

        await _dbContext.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id)
    {
        if (await AnyAsync(id) is false)
            throw new InvalidDataException(id.ToString());

        var eventEntity = new BookingEvent
        {
            StreamId = id,
            Type = EventType.Delete,
            Name = "Deleted",
            DateTime = DateTime.Now,
        };

        _dbContext.BookingEvents.Add(eventEntity);

        await _dbContext.SaveChangesAsync();
    }



    private BookingEntity Mutate(IEnumerable<BookingEvent> events)
    {
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

    private async Task<int> GetLastStreamIdAsync()
    {
        return await _dbContext.BookingEvents.OrderByDescending(be => be.StreamId)
            .Select(be => be.StreamId).FirstOrDefaultAsync();
    }

    private async Task<bool> AnyAsync(int id)
        => await _dbContext.BookingEvents.AnyAsync(be => be.StreamId == id);
}