using System;
using LT.DigitalOffice.EventService.Mappers.Db.Interfaces;
using LT.DigitalOffice.EventService.Models.Db;
using LT.DigitalOffice.EventService.Models.Dto.Requests.Category;
using LT.DigitalOffice.Kernel.Extensions;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.EventService.Mappers.Db
{
  public class DbCategoryMapper : IDbCategoryMapper
  {
    private readonly IHttpContextAccessor _contextAccessor;

    public DbCategoryMapper(IHttpContextAccessor accessor)
    {
      _contextAccessor = accessor;
    }

    public DbCategory Map(CreateCategoryRequest request)
    {
      return request is null
        ? null
        : new DbCategory
        {
          Id = Guid.NewGuid(),
          IsActive = true,
          Name = request.Name,
          Color = request.Color,
          CreatedBy = _contextAccessor.HttpContext.GetUserId(),
          CreatedAtUtc = DateTime.UtcNow
        };
    }
  }
}
