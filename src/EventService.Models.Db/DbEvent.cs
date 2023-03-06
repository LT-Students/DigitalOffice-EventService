using System;
using System.Collections.Generic;
using LT.DigitalOffice.EventService.Models.Dto.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.EventService.Models.Db;

public class DbEvent
{
  public const string TableName = "Events";

  public Guid Id { get; set; }
  public string Name { get; set; }
  public string Address { get; set; }
  public string Description { get; set; }
  public DateTime Date { get; set; }
  public FormatType Format { get; set; }
  public AccessType Access { get; set; }
  public bool IsActive { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTime CreatedAtUtc { get; set; }
  public Guid? ModifiedBy { get; set; }
  public DateTime? ModifiedAtUtc { get; set; }

  public ICollection<DbEventCategory> EventCategories { get; set; }
  public ICollection<DbEventFile> Files { get; set; }
  public ICollection<DbEventImage> Images { get; set; }
  public ICollection<DbEventUser> EventUsers { get; set; }
  public ICollection<DbEventComment> Comments { get; set; }

  public DbEvent()
  {
    EventCategories = new HashSet<DbEventCategory>();
    Files = new HashSet<DbEventFile>();
    Images = new HashSet<DbEventImage>();
    EventUsers = new HashSet<DbEventUser>();
    Comments = new HashSet<DbEventComment>();
  }

  public class DbEventConfiguration : IEntityTypeConfiguration<DbEvent>
  {
    public void Configure(EntityTypeBuilder<DbEvent> builder)
    {
      builder
        .ToTable(DbEvent.TableName);

      builder
        .HasKey(t => t.Id);

      builder
        .HasMany(e => e.EventCategories)
        .WithOne(ec => ec.Event);

      builder
        .HasMany(e => e.Files)
        .WithOne(ef => ef.Event);

      builder
        .HasMany(e => e.Images)
        .WithOne(ei => ei.Event);

      builder
        .HasMany(e => e.EventUsers)
        .WithOne(eu => eu.Event);
    }
  }
}
