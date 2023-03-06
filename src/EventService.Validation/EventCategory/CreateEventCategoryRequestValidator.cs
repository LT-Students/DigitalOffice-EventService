using System.Linq;
using FluentValidation;
using LT.DigitalOffice.EventService.Data.Interfaces;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventCategory;
using LT.DigitalOffice.EventService.Validation.EventCategory.Interfaces;

namespace LT.DigitalOffice.EventService.Validation.EventCategory;

public class CreateEventCategoryRequestValidator : AbstractValidator<CreateEventCategoryRequest>, ICreateEventCategoryRequestValidator
{
  public CreateEventCategoryRequestValidator(
    IEventRepository eventRepository,
    ICategoryRepository categoryRepository,
    IEventCategoryRepository eventCategoryRepository)
  {
    RuleFor(x => x.EventId)
      .MustAsync(async (eventId, _) => await eventRepository.DoesExistAsync(eventId))
      .WithMessage("This event doesn't exist.");

    RuleFor(x => x.CategoriesIds)
      .Must(categoryRepository.DoesExistAllAsync)
      .WithMessage("Some of categories in the list doesn't exist.");

    RuleFor(x => x)
      .Must(ec=>!eventCategoryRepository.DoesExistAsync(ec.EventId, ec.CategoriesIds))
      .WithMessage("This event already belongs to this category.")
      .MustAsync(async (ec, _) => await eventCategoryRepository.CountCategoriesAsync(ec.EventId) + ec.CategoriesIds.Count < 6)
      .WithMessage("This event already has 5 categories.");
  }
}
