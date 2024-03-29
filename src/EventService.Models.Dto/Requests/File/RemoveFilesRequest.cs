﻿using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.EventService.Models.Dto.Requests.File;

public record RemoveFilesRequest
{
  public Guid EntityId { get; set; }
  public List<Guid> FilesIds { get; set; }
}
