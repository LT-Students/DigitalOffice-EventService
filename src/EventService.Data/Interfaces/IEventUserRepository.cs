﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.EventService.Models.Db;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventUser.Filter;
using LT.DigitalOffice.Kernel.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.EventService.Data.Interfaces;

[AutoInject]
public interface IEventUserRepository
{
  Task<bool> DoesExistAsync(List<Guid> userId, Guid eventId);
  Task<bool> DoesExistAsync(Guid eventUserId);
  Task<bool> CreateAsync(List<DbEventUser> dbEventUsers);
  Task<List<DbEventUser>> FindAsync(
    Guid eventId, 
    FindEventUsersFilter filter, 
    CancellationToken cancellationToken);
  Task<DbEventUser> GetAsync(Guid eventUserId);
  Task<bool> EditAsync(Guid eventUserId, JsonPatchDocument<DbEventUser> request);
}
