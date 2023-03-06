using FluentValidation;
using LT.DigitalOffice.EventService.Data.Interfaces;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventCategory;
using LT.DigitalOffice.EventService.Validation.EventCategory.Interfaces;

namespace LT.DigitalOffice.EventService.Validation.EventCategory;

public class RemoveEventCategoryRequestValidator : AbstractValidator<RemoveEventCategoryRequest>, IRemoveEventCategoryRequestValidator
{
  public RemoveEventCategoryRequestValidator(
    IEventRepository eventRepository,
    ICategoryRepository categoryRepository,
    IEventCategoryRepository eventCategoryRepository)
  {
    RuleFor(request => request.EventId)
      .MustAsync(async (x, _) => await eventRepository.DoesExistAsync(x))
      .WithMessage("This event doesn't exist.");

    RuleFor(request => request.CategoriesIds)
      .NotEmpty()
      .WithMessage("There are no categories to delete.")
      .Must(categoryRepository.DoesExistAllAsync)
      .WithMessage("Some categories doesn't exist.");

    RuleFor(request => request)
      .Must(x => eventCategoryRepository.DoesExistAsync(x.EventId, x.CategoriesIds))
      .WithMessage("This event doesn't belong to all categories in the list.");
  }
}
