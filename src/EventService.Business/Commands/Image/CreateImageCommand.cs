﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Models.Image;
using FluentValidation.Results;
using LT.DigitalOffice.EventService.Broker.Requests.Interfaces;
using LT.DigitalOffice.EventService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.EventService.Data.Interfaces;
using LT.DigitalOffice.EventService.Mappers.Db.Interfaces;
using LT.DigitalOffice.EventService.Models.Dto.Requests.Image;
using LT.DigitalOffice.EventService.Validation.Image.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.EventService.Business.Commands.Image;

public class CreateImageCommand : ICreateImageCommand
{
  private readonly IImageRepository _repository;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IDbImageMapper _dbImageMapper;
  private readonly ICreateImagesRequestValidator _validator;
  private readonly IResponseCreator _responseCreator;
  private readonly IImageService _imageService;

  private const int ResizeMaxValue = 1000;
  private const int ConditionalWidth = 4;
  private const int ConditionalHeight = 3;

  public CreateImageCommand(
    IImageRepository repository,
    IHttpContextAccessor httpContextAccessor,
    IDbImageMapper dbImageMapper,
    ICreateImagesRequestValidator validator,
    IResponseCreator responseCreator,
    IImageService imageService)
  {
    _repository = repository;
    _httpContextAccessor = httpContextAccessor;
    _dbImageMapper = dbImageMapper;
    _validator = validator;
    _responseCreator = responseCreator;
    _imageService = imageService;
  }

  public async Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateImagesRequest request)
  {
    ValidationResult validationResult = await _validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<List<Guid>>(
        HttpStatusCode.BadRequest,
        validationResult.Errors.ConvertAll(x => x.ErrorMessage));
    }

    OperationResultResponse<List<Guid>> response = new();

    List<Guid> imagesIds = await _imageService.CreateImagesAsync(
      request.Images,
      new ResizeParameters(
        maxResizeValue: ResizeMaxValue,
        maxSizeCompress: null,
        previewParameters: new PreviewParameters(
          conditionalWidth: ConditionalWidth,
          conditionalHeight: ConditionalHeight,
          resizeMaxValue: null,
          maxSizeCompress: null)),
      response.Errors);

    if (response.Errors.Any())
    {
      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

      return response;
    }

    response.Body = await _repository.CreateAsync(imagesIds.ConvertAll(imageId =>
      _dbImageMapper.Map(
        imageId: imageId,
        entityId: request.EntityId)));

    _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

    return response;
  }
}
