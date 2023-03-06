using FluentValidation;
using LT.DigitalOffice.EventService.Models.Dto.Requests.Event;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.EventService.Validation.Event.Interfaces;

[AutoInject]
public interface ICreateEventRequestValidator : IValidator<CreateEventRequest>
{
}
