using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LT.DigitalOffice.EventService.Data.Provider.MsSql.Ef;
using HealthChecks.UI.Client;
using LT.DigitalOffice.EventService.Models.Dto.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Extensions;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.BrokerSupport.Middlewares.Token;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.EFSupport.Extensions;
using LT.DigitalOffice.Kernel.EFSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.EventService;

public class Startup : BaseApiInfo
{
  private readonly BaseServiceInfoConfig _serviceInfoConfig;
  private readonly RabbitMqConfig _rabbitMqConfig;

  public const string CorsPolicyName = "LtDoCorsPolicy";
  public IConfiguration Configuration { get; }

  private void ConfigureMassTransit(IServiceCollection services)
  {
    (string username, string password) = RabbitMqCredentialsHelper
      .Get(_rabbitMqConfig, _serviceInfoConfig);

    services.AddMassTransit(busConfigurator =>
    {
      busConfigurator.UsingRabbitMq((context, cfg) =>
      {
        cfg.Host(_rabbitMqConfig.Host, "/", host =>
        {
          host.Username(username);
          host.Password(password);
        });
      });

      busConfigurator.AddRequestClients(_rabbitMqConfig);
    });

    services.AddMassTransitHostedService();
  }

  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;

    _serviceInfoConfig = Configuration
      .GetSection(BaseServiceInfoConfig.SectionName)
      .Get<BaseServiceInfoConfig>();

    _rabbitMqConfig = Configuration
      .GetSection(BaseRabbitMqConfig.SectionName)
      .Get<RabbitMqConfig>();

    Version = "1.0.0.0";
    Description = "EventService is an API that intended to work with events.";
    StartTime = DateTime.UtcNow;
    ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddCors(options =>
    {
      options.AddPolicy(
        CorsPolicyName,
        builder =>
        {
          builder
          .AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
        });
    });

    string dbConnStr = ConnectionStringHandler.Get(Configuration);

    services.AddDbContext<EventServiceDbContext>(options =>
    {
      options.UseSqlServer(dbConnStr);
    });

    services.AddHttpContextAccessor();

    services.AddHealthChecks()
      .AddRabbitMqCheck()
      .AddSqlServer(dbConnStr);

    services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
    services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));
    services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));

    services.AddBusinessObjects();

    ConfigureMassTransit(services);

    services.AddControllers()
      .AddJsonOptions(options =>
     {
       options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     })
      .AddNewtonsoftJson();
  }

  public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
  {
    app.UpdateDatabase<EventServiceDbContext>();

    app.UseForwardedHeaders();

    app.UseExceptionsHandler(loggerFactory);

    app.UseApiInformation();

    app.UseRouting();

    app.UseMiddleware<TokenMiddleware>();

    app.UseCors(CorsPolicyName);

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers().RequireCors(CorsPolicyName);
      endpoints.MapHealthChecks($"/{_serviceInfoConfig.Id}/hc", new HealthCheckOptions
      {
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
          { HealthStatus.Unhealthy, 200 },
          { HealthStatus.Healthy, 200 },
          { HealthStatus.Degraded, 200 },
        },
        Predicate = check => check.Name != "masstransit-bus",
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
      });
    });
  }
}
