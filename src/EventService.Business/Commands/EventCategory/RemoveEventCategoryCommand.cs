using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.EventService.Business.Commands.EventCategory.Interfaces;
using LT.DigitalOffice.EventService.Data.Interfaces;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventCategory;
using LT.DigitalOffice.EventService.Validation.EventCategory.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.EventService.Business.Commands.EventCategory;

public class RemoveEventCategoryCommand : IRemoveEventCategoryCommand
{
  private readonly IHttpContextAccessor _contextAccessor;
  private readonly IAccessValidator _accessValidator;
  private readonly IEventCategoryRepository _repository;
  private readonly IRemoveEventCategoryRequestValidator _validator;
  private readonly IResponseCreator _responseCreator;

  public RemoveEventCategoryCommand(
    IHttpContextAccessor contextAccessor,
    IAccessValidator accessValidator,
    IEventCategoryRepository repository,
    IRemoveEventCategoryRequestValidator validator,
    IResponseCreator responseCreator)
  {
    _contextAccessor = contextAccessor;
    _accessValidator = accessValidator;
    _repository = repository;
    _validator = validator;
    _responseCreator = responseCreator;
  }

  public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveEventCategoryRequest request)
  {
    if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
    }

    ValidationResult validationResult = await _validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.BadRequest,
        validationResult.Errors.Select(er => er.ErrorMessage).ToList());
    }

    OperationResultResponse<bool> response = new()
    {
      Body = await _repository.RemoveAsync(request.EventId, request.CategoriesIds)
    };

    if (!response.Body)
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
    }

    _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

    return response;
  }
}
