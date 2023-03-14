using LT.DigitalOffice.EventService.Models.Db;
using LT.DigitalOffice.EventService.Models.Dto.Requests.Category;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.EventService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbCategoryMapper
  {
    DbCategory Map(CreateCategoryRequest request);
  }
}
