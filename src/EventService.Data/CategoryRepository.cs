using System;
using System.Linq;
using LT.DigitalOffice.EventService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.EventService.Data.Interfaces;
using LT.DigitalOffice.EventService.Data.Provider;

namespace LT.DigitalOffice.EventService.Data;

public class CategoryRepository : ICategoryRepository
{
  private readonly IDataProvider _provider;

  public CategoryRepository(IDataProvider provider)
  {
    _provider = provider;
  }

  public bool DoesExistAllAsync(List<Guid> categoriesIds)
  {
    return categoriesIds.All(categoryId =>
      _provider.Categories.AsNoTracking().AnyAsync(c => c.Id == categoryId && c.IsActive).Result);
  }

  public async Task<Guid?> CreateAsync(DbCategory dbCategory)
  {
    if (dbCategory is null)
    {
      return null;
    }

    _provider.Categories.Add(dbCategory);
    await _provider.SaveAsync();

    return dbCategory.Id;
  }
}

