
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Services;
using System.Configuration;
using System.Xml.Linq;
using TradeAggreatorService;
using TradeAggreatorService.Interface;
using TradeAggreatorService.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((hostContext, config) =>
    {
        config.AddLog4Net("log4net.config");
    })
    .ConfigureServices(services =>
    {
        JobKey jobIdentifier = new JobKey("hourly-trade-aggregator");
        var minutes = 5;
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(AppContext.BaseDirectory))
            .AddJsonFile("appsettings.json", optional: true);
        builder.AddEnvironmentVariables();
        IConfiguration config = builder.Build();
        int.TryParse(config["App:IntervalInMins"], out minutes);


        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.AddJob<AggregationJob>(config =>
            {
                config.WithIdentity(jobIdentifier);
            });
            q.AddTrigger(config =>
            config.WithIdentity("trigger")
            .WithCronSchedule($"0 0/{minutes} * * * ?")
            .ForJob(jobIdentifier)
            .StartNow());
            q.AddTrigger(config => config.WithIdentity("startup")
                            .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForTotalCount(1))
                            .ForJob(jobIdentifier)
                            .StartNow());

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        });
        services.AddScoped<IPowerService, PowerService>();
        services.AddScoped<IAggregator, AggregatorService>();
        services.AddScoped<IExportReport, CsvGeneratorService>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();