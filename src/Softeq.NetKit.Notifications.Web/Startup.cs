// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using CorrelationId;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Softeq.NetKit.Notifications.Web.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using Softeq.NetKit.Notifications.Web.Infrastructure.ErrorHandling;

namespace Softeq.NetKit.Notifications.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAsyncInitialization();

            if (bool.TryParse(Configuration["Notifications:Storage:RunBootstrapper"], out var runBootstrapper) && runBootstrapper)
            {
                services.AddAsyncInitializer<StorageInitializer>();
            }

            services.AddMvcCore(o =>
                {
                    o.Filters.Add(typeof(GlobalExceptionFilter));

                    if (!_hostingEnvironment.IsDevelopment())
                    {
                        o.Filters.Add(typeof(RequireHttpsAttribute));
                    }
                })
                .AddApiExplorer()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiSecret = Configuration["Authentication:Secret"];
                    options.Authority = Configuration["Authentication:Authority"];
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "api";
                });

            //services.AddAutoMapper(typeof(Service.ContainerModule).Assembly,
            //    typeof(Store.CosmosDB.ContainerModule).Assembly);

            services.AddAutoMapper(typeof(Service.ContainerModule).Assembly,
                typeof(Store.Sql.ContainerModule).Assembly);

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddMemoryCache();

            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Softeq NetKit Notification API",
                    Description = "Softeq NetKit Notification API"
                });
                c.DescribeAllEnumsAsStrings();

                var path = Path.Combine("wwwroot", @"api.xml");
                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }
            });

            ConfigureAdditionalServices(services);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterAssemblyModules(typeof(Startup).Assembly);

            ApplicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown, app);

            app.UseCors(x => x.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            if (_hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(options =>
                {
                    options.Run(async c => await ExceptionHandler.Handle(c, loggerFactory));
                });
            }

            app.UseCorrelationId();

            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Softeq NetKit Notification API");
            });

            ConfigureAdditionalMiddleware(app);

            app.UseMvc();
        }

        protected virtual void ConfigureAdditionalMiddleware(IApplicationBuilder app)
        {
        }

        protected virtual void ConfigureAdditionalServices(IServiceCollection services)
        {
        }

        private void OnShutdown(object builder)
        {
            if (builder is IApplicationBuilder applicationBuilder)
            {
                var telemetryClient = applicationBuilder.ApplicationServices.GetService<TelemetryClient>();
                if (telemetryClient != null)
                {
                    telemetryClient.Flush();
                    //Wait while the data is flushed
                    System.Threading.Thread.Sleep(1000);
                }
                Log.CloseAndFlush();
            }
        }
    }
}
