using FluentValidation;
using LT.DigitalOffice.EventService.Models.Dto.Requests.Category;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.EventService.Validation.Category.Interfaces;

[AutoInject]
public interface ICreateCategoryRequestValidator : IValidator<CreateCategoryRequest>
{
}

