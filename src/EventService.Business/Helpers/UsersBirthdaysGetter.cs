﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Models.User;
using LT.DigitalOffice.EventService.Broker.Requests.Interfaces;
using LT.DigitalOffice.EventService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.EventService.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LT.DigitalOffice.EventService.Business.Helpers
{
  public class UsersBirthdaysGetter
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUserService _userService;

    private async Task ExecuteAsync()
    {
      using var scope = _scopeFactory.CreateScope();
      using var dbContext = scope.ServiceProvider.GetRequiredService<EventServiceDbContext>();

      List<UserBirthday> usersBirthdays = await _userService.GetUsersBirthdaysAsync();

      if (usersBirthdays is null || !usersBirthdays.Any())
      {
        return;
      }

      List<Guid> users = await dbContext.UsersBirthdays.Where(ub => ub.IsActive).Select(ub => ub.UserId).ToListAsync();

      dbContext.UsersBirthdays.AddRange(
        usersBirthdays.Where(ub => !users.Contains(ub.UserId)).Select(ub => new DbUserBirthday
        {
          UserId = ub.UserId,
          DateOfBirthday = ub.DateOfBirth,
          IsActive = true,
          CreatedAtUtc = DateTime.UtcNow
        }));

      await dbContext.SaveChangesAsync();
    }

    public UsersBirthdaysGetter(
      IServiceScopeFactory scopeFactory,
      IUserService userService)
    {
      _scopeFactory = scopeFactory;
      _userService = userService;
    }

    public void Start()
    {
      Task.Run(async () =>
      {
        await ExecuteAsync();
      });
    }
  }
}