using System;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.EventService.Business.Commands.EventUser.Interfaces;
using LT.DigitalOffice.EventService.Models.Dto.Models;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventUser;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventUser.Filter;
using LT.DigitalOffice.Kernel.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.EventService.Controllers;

[Route("[controller]")]
[ApiController]
public class EventUserController : ControllerBase
{
  [HttpPost("create")]
  public async Task<OperationResultResponse<bool>> CreateAsync(
    [FromServices] ICreateEventUserCommand command,
    [FromBody] CreateEventUserRequest request)
  {
    return await command.ExecuteAsync(request);
  }

  [HttpGet("find")]
  public async Task<FindResultResponse<EventUserInfo>> FindAsync(
    [FromServices] IFindEventUserCommand command,
    [FromQuery] Guid eventId,
    [FromQuery] FindEventUsersFilter filter,
    CancellationToken cancellationToken)
  {
    return await command.ExecuteAsync(eventId: eventId, filter: filter, cancellationToken: cancellationToken);
  }
}
