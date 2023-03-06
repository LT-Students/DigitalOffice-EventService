using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.EventService.Mappers.Db.Interfaces;
using LT.DigitalOffice.EventService.Models.Db;
using LT.DigitalOffice.EventService.Models.Dto.Enums;
using LT.DigitalOffice.EventService.Models.Dto.Requests.Event;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventUser;

namespace LT.DigitalOffice.EventService.Mappers.Db;

public class DbEventUserMapper : IDbEventUserMapper
{
  public List<DbEventUser> Map(
    CreateEventUserRequest request, 
    AccessType access, 
    Guid senderId)
  {
    return request is null
      ? null
      : request.Users.Select(u => new DbEventUser
      {
        Id = Guid.NewGuid(),
        EventId = request.EventId,
        UserId = u.UserId,
        Status = (access == AccessType.Opened && u.UserId == senderId)
          ? EventUserStatus.Participant
          : EventUserStatus.Invited,
        NotifyAtUtc = u.NotifyAtUtc,
        CreatedBy = senderId,
        CreatedAtUtc = DateTime.UtcNow
      }).ToList();
  }
}
