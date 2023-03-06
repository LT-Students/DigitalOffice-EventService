using System;
using System.Threading.Tasks;
using LT.DigitalOffice.EventService.Data.Provider;
using LT.DigitalOffice.EventService.Data.Interfaces;
using LT.DigitalOffice.EventService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.EventService.Data;

public class EventRepository : IEventRepository
{
  private readonly IDataProvider _provider;

  public EventRepository(IDataProvider provider)
  {
    _provider = provider;
  }

  public async Task<bool> CreateAsync(DbEvent dbEvent)
  {
    if (dbEvent is null)
    {
      return false;
    }

    _provider.Events.Add(dbEvent);
    _provider.EventsUsers.AddRange(dbEvent.EventUsers);
    _provider.EventsCategories.AddRange(dbEvent.EventCategories);
    await _provider.SaveAsync();

    return true;
  }
  public Task<bool> DoesExistAsync(Guid eventId)
  {
    return _provider.Events.AsNoTracking().AnyAsync(e => e.Id == eventId && e.IsActive);
  }

  public async Task<DbEvent> GetAsync(Guid eventId)
  {
    return await _provider.Events.FirstOrDefaultAsync(e => e.Id == eventId);
  }
}
