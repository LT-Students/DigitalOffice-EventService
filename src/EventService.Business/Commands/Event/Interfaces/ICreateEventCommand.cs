using System;
using System.Threading.Tasks;
using LT.DigitalOffice.EventService.Models.Dto.Requests.Event;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.EventService.Business.Commands.Event.Interfaces;

[AutoInject]
public interface ICreateEventCommand
{
  Task<OperationResultResponse<bool>> ExecuteAsync(CreateEventRequest request);
}
