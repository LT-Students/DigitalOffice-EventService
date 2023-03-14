using System;
using System.Collections.Generic;
using LT.DigitalOffice.EventService.Models.Dto.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.EventService.Models.Db;

public class DbCategory
{
  public const string TableName = "Categories";

  public Guid Id { get; set; }
  public string Name { get; set; }
  public CategoryColor Color { get; set; }
  public bool IsActive { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTime CreatedAtUtc { get; set; }
  public Guid? ModifiedBy { get; set; }
  public DateTime? ModifiedAtUtc { get; set; }

  public ICollection<DbEventCategory> EventsCategories { get; set; }
}

public class DbCategoryConfiguration : IEntityTypeConfiguration<DbCategory>
{
  public void Configure(EntityTypeBuilder<DbCategory> builder)
  {
    builder
      .ToTable(DbCategory.TableName);

    builder
      .HasKey(t => t.Id);

    builder
      .HasMany(e => e.EventsCategories)
      .WithOne(ec => ec.Category);
  }
}
