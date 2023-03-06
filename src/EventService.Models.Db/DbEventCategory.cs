using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.EventService.Models.Db;

public class DbEventCategory
{
  public const string TableName = "EventsCategories";

  public Guid Id { get; set; }
  public Guid EventId { get; set; }
  public Guid CategoryId { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTime CreatedAtUtc { get; set; }

  public DbEvent Event { get; set; }
  public DbCategory Category { get; set; }
}

public class DbEventCategoryConfiguration : IEntityTypeConfiguration<DbEventCategory>
{
  public void Configure(EntityTypeBuilder<DbEventCategory> builder)
  {
    builder
      .ToTable(DbEventCategory.TableName);

    builder
      .HasKey(t => t.Id);

    builder
      .HasOne(ec => ec.Event)
      .WithMany(e => e.EventCategories);

    builder
      .HasOne(ec => ec.Category)
      .WithMany(c => c.EventsCategories);
  }
}
