using System;
using LT.DigitalOffice.EventService.Models.Dto.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.EventService.Models.Db;

public class DbEventUser
{
  public const string TableName = "EventsUsers";

  public Guid Id { get; set; }
  public Guid EventId { get; set; }
  public Guid UserId { get; set; }
  public EventUserStatus Status { get; set; }
  public DateTime? NotifyAtUtc { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTime CreatedAtUtc { get; set; }
  public Guid? ModifiedBy { get; set; }
  public DateTime? ModifiedAtUtc { get; set; }

  public DbEvent Event { get; set; }
}

public class DbEventUserConfiguration : IEntityTypeConfiguration<DbEventUser>
{
  public void Configure(EntityTypeBuilder<DbEventUser> builder)
  {
    builder
      .ToTable(DbEventUser.TableName);

    builder
      .HasKey(t => t.Id);

    builder
      .HasOne(eu => eu.Event)
      .WithMany(e => e.EventUsers);
  }
}
