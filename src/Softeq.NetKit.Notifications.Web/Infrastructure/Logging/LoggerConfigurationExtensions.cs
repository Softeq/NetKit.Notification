// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using CorrelationId;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Serilog.ExtensionMethods;
using Softeq.Serilog.Extension;
using ILogger = Serilog.ILogger;

namespace Softeq.NetKit.Notifications.Web.Infrastructure.Logging
{
    internal static class LoggerConfigurationExtensions
    {
        public static IWebHostBuilder UseSerilog(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((context, collection) => collection.AddLogging(builder => SetupSerilog(context, collection)));
        }

        private static void SetupSerilog(WebHostBuilderContext hostingContext, IServiceCollection serviceCollection)
        {
            var applicationName = hostingContext.Configuration["Serilog:ApplicationName"];
            var environment = hostingContext.HostingEnvironment.EnvironmentName;
            var applicationSlotName = $"{applicationName}:{environment}";

            var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(hostingContext.Configuration)
                                                               .Enrich.WithProperty(PropertyNames.Application, applicationSlotName);

            var template = GetLogTemplate();

            if (hostingContext.HostingEnvironment.IsProduction() ||
                hostingContext.HostingEnvironment.IsStaging() ||
                hostingContext.HostingEnvironment.IsDevelopment())
            {
                var instrumentationKey = hostingContext.Configuration["ApplicationInsights:InstrumentationKey"];
                if (string.IsNullOrWhiteSpace(instrumentationKey))
                {
                    var telemetryClient = new TelemetryClient { InstrumentationKey = instrumentationKey };
                    loggerConfiguration.WriteTo.ApplicationInsights(telemetryClient, 
                        (@event, provider) => LogEventsToTelemetryConverter(@event, provider));
                    loggerConfiguration.WriteTo.Debug(LogEventLevel.Debug);
                    serviceCollection.AddSingleton(telemetryClient);
                }
            }
            else
            {
                loggerConfiguration.WriteTo.Debug(outputTemplate: template);
            }

            bool.TryParse(hostingContext.Configuration["Serilog:EnableLocalFileSink"], out var localFileSinkEnabled);
            if (localFileSinkEnabled)
            {
                var fileSizeInBytes = int.Parse(hostingContext.Configuration["Serilog:FileSizeLimitMBytes"]) * 1024 * 1024;
                loggerConfiguration.WriteTo.RollingFile("logs/log-{Date}.txt", outputTemplate: template, fileSizeLimitBytes: fileSizeInBytes);
            }

            var logger = loggerConfiguration.CreateLogger();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                logger.ErrorEvent("UnhandledExceptionCaughtByAppDomainUnhandledExceptionHandler", "{ExceptionObject} {IsTerminating}", args.ExceptionObject, args.IsTerminating);
            };

            Log.Logger = logger;

            serviceCollection.AddScoped<ILogger>(provider =>
            {
                var correlationContextAccessor = provider.GetService<ICorrelationContextAccessor>();
                return Log.Logger
                    .ForContext(new CorrelationIdEnricher(correlationContextAccessor))
                    .ForContext(new UserContextEnricher());
            });

            serviceCollection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, true));
        }

        private static ITelemetry LogEventsToTelemetryConverter(LogEvent serilogLogEvent, IFormatProvider formatProvider)
        {
            if (serilogLogEvent.Exception != null)
            {
                return serilogLogEvent.ToDefaultExceptionTelemetry(formatProvider);
            }

            if (serilogLogEvent.Properties.ContainsKey(PropertyNames.EventId))
            {
                var eventTelemetry = new EventTelemetry(serilogLogEvent.Properties[PropertyNames.EventId].ToString())
                {
                    Timestamp = serilogLogEvent.Timestamp
                };
                serilogLogEvent.ForwardPropertiesToTelemetryProperties(eventTelemetry, formatProvider);
                return eventTelemetry;
            }

            var exceptionTelemetry = new ExceptionTelemetry(new Exception($"Event does not contain '{PropertyNames.EventId}' property"))
            {
                Timestamp = serilogLogEvent.Timestamp
            };
            serilogLogEvent.ForwardPropertiesToTelemetryProperties(exceptionTelemetry, formatProvider);
            return exceptionTelemetry;
        }

        private static string GetLogTemplate()
        {
            var template = new SerilogTemplateBuilder().Timestamp()
                .Level()
                .CorrelationId()
                .EventId()
                .AddPlaceholder("UserId")
                .Message()
                .NewLine()
                .Exception()
                .Build();
            return template;
        }
    }
}
