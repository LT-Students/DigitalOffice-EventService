using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.EventService.Broker.Requests.Interfaces;
using LT.DigitalOffice.EventService.Business.Commands.EventUser.Interfaces;
using LT.DigitalOffice.EventService.Data.Interfaces;
using LT.DigitalOffice.EventService.Mappers.Db.Interfaces;
using LT.DigitalOffice.EventService.Models.Db;
using LT.DigitalOffice.EventService.Models.Dto.Enums;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventUser;
using LT.DigitalOffice.EventService.Validation.EventUser.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.EventService.Business.Commands.EventUser;

public class CreateEventUserCommand : ICreateEventUserCommand
{
  private readonly IAccessValidator _accessValidator;
  private readonly IEventUserRepository _repository;
  private readonly IDbEventUserMapper _mapper;
  private readonly ICreateEventUserRequestValidator _validator;
  private readonly IResponseCreator _responseCreator;
  private readonly IHttpContextAccessor _contextAccessor;
  private readonly IEventRepository _eventRepository;
  private readonly IEmailService _emailService;
  private readonly IUserService _userService;
  
  public CreateEventUserCommand(
    IAccessValidator accessValidator,
    IEventUserRepository repository,
    IDbEventUserMapper mapper,
    ICreateEventUserRequestValidator validator,
    IResponseCreator responseCreator,
    IHttpContextAccessor contextAccessor,
    IEventRepository eventRepository,
    IEmailService emailService,
    IUserService userService)
  {
    _accessValidator = accessValidator;
    _repository = repository;
    _mapper = mapper;
    _validator = validator;
    _responseCreator = responseCreator;
    _contextAccessor = contextAccessor;
    _eventRepository = eventRepository;
    _emailService = emailService;
    _userService = userService;
  }

  private async Task SendInviteEmailsAsync(List<Guid> userIds, string eventName)
  {
    List<UserData> usersData = await _userService.GetUsersDataAsync(userIds);

    if (usersData is null || !usersData.Any())
    {
      return;
    }

    foreach (UserData user in usersData)
    {
      await _emailService.SendAsync(
        user.Email,
        "Invite to event",
        $"You have been invited to event {eventName}");
    }
  }

  public async Task<OperationResultResponse<bool>> ExecuteAsync(CreateEventUserRequest request)
  {
    Guid senderId = _contextAccessor.HttpContext.GetUserId();
    DbEvent dbEvent = await _eventRepository.GetAsync(request.EventId);

    if (dbEvent is null)
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.NotFound,
        new List<string> { "This event doesn't exist." });
    }

    if ((dbEvent.Access == AccessType.Closed && !await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers))
        || !(dbEvent.Access == AccessType.Opened
          && (await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers)
            || (!await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers)
              && request.Users.Count == 1
              && request.Users.Exists(x => x.UserId == senderId)))))
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
    }

    OperationResultResponse<bool> response = new();

    if (request.Users.Distinct().Count() != request.Users.Count())
    {
      response.Errors = new List<string>() { "Some duplicate users have been removed from the list." };
      request.Users = request.Users.Distinct().ToList();
    }

    ValidationResult validationResult = await _validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.BadRequest,
        validationResult.Errors.Select(er => er.ErrorMessage).ToList());
    }

    List<DbEventUser> dbEventUsers = _mapper.Map(request, dbEvent.Access, senderId);
    response.Body = await _repository.CreateAsync(dbEventUsers);
    _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

    await SendInviteEmailsAsync(dbEventUsers.Where(x => x.Status == EventUserStatus.Invited).Select(x => x.UserId).ToList(), dbEvent.Name);

    return response;
  }
}
