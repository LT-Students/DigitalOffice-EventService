using System;
using System.Threading.Tasks;
using LT.DigitalOffice.EventService.Models.Db;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.EventService.Data.Interfaces;

[AutoInject]
public interface IEventRepository
{
  Task<bool> CreateAsync(DbEvent dbEvent);
  Task<bool> DoesExistAsync(Guid eventId);
  Task<DbEvent> GetAsync(Guid eventId);
}
