﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.EventService.Models.Db;
using LT.DigitalOffice.Kernel.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.EventService.Data.Interfaces;

[AutoInject]
public interface IEventRepository
{
  Task<Guid?> CreateAsync(DbEvent dbEvent);
  Task<bool> EditAsync(Guid eventId, JsonPatchDocument<DbEvent> request);
  Task<bool> DoesExistAsync(Guid eventId, bool? isActive);
  public Task<bool> IsEventCompletedAsync(Guid eventId);
  Task<List<Guid>> GetExisting(List<Guid> eventsIds);
  Task<DbEvent> GetAsync(Guid eventId);
}
