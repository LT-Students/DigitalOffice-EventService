﻿using System.Linq;
using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Publishing.Subscriber.File;
using LT.DigitalOffice.EventService.Data.Interfaces;
using LT.DigitalOffice.EventService.Mappers.Db.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.EventService.Broker.Consumers;

public class CreateFilesConsumer : IConsumer<ICreateEventFilesPublish>
{
  private readonly IFileRepository _repository;
  private readonly IDbFileMapper _mapper;

  public CreateFilesConsumer(
    IFileRepository repository,
    IDbFileMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task Consume(ConsumeContext<ICreateEventFilesPublish> context)
  {
    if (context.Message.FilesIds is not null && context.Message.FilesIds.Any())
    {
      await _repository.CreateAsync(context.Message.FilesIds
        .ConvertAll(x => _mapper.Map(x, context.Message.EventId)));
    }
  }
}
