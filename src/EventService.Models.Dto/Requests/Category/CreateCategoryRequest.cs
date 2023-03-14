using System;
using System.ComponentModel.DataAnnotations;
using LT.DigitalOffice.EventService.Models.Dto.Enums;

namespace LT.DigitalOffice.EventService.Models.Dto.Requests.Category;

public class CreateCategoryRequest
{
  [Required]
  public string Name { get; set; }
  public CategoryColor Color { get; set; }
}

