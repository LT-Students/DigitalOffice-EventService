using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LT.DigitalOffice.EventService.Models.Dto.Enums;
using LT.DigitalOffice.EventService.Models.Dto.Requests.EventUser;

namespace LT.DigitalOffice.EventService.Models.Dto.Requests.Event;

public record CreateEventRequest
{
  [Required]
  public string Name { get; set; }
  [Required]
  public string Address { get; set; }
  public string Description { get; set; }
  public DateTime Date { get; set; }
  public FormatType Format { get; set; }
  public AccessType Access { get; set; }
  [Required]
  public List<UserRequest> Users { get; set; }
  [Required]
  public List<Guid> CategoriesIds { get; set; }
}
