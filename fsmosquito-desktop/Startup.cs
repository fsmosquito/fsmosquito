namespace FsMosquito
{
    using FsMosquito.Extensions;
    using FsMosquito.Mqtt;
    using FsMosquito.Models;
    using FsMosquito.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using MQTTnet.AspNetCore;
    using MQTTnet.AspNetCore.Extensions;
    using System;
    using System.IO;
    using System.Reflection;
    using Serilog;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var targetLogsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "fsmosquito-desktop-logs");
            if (!Directory.Exists(targetLogsPath))
            {
                Directory.CreateDirectory(targetLogsPath);
            }

            services.AddLogging(builder =>
            {
                builder
                .AddSerilog()
                .SetMinimumLevel(LogLevel.Information)
                .AddFile($"{targetLogsPath}/FsMosquitoDesktop-{{Date}}.txt")
                .AddConfiguration(Configuration.GetSection("Logging"));
            });

            services.AddSingleton<IMimeMappingService, MimeMappingService>((sp) => {
                var provider = new FileExtensionContentTypeProvider();
                return new MimeMappingService(provider);
            });

            services.AddHttpClient();

            services.Configure<LiteDbOptions>(Configuration.GetSection("LiteDbOptions"));
            services.AddSingleton<ILiteDbContext, LiteDbContext>();

            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddControllers();

            services.AddSwaggerGen(config =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

                config.AddSecurityDefinition("ApiKeyAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "X-API-KEY"
                });

                config.OperationFilter<ApiKeyOperationFilter>();
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "../fsmosquito-app/out";
            });

            services.AddSingleton<IMqttService, MqttService>();

            services
                .AddHostedMqttServerWithServices(serverOptionsBuilder =>
                {
                    serverOptionsBuilder.WithoutDefaultEndpoint();
                    serverOptionsBuilder.WithSubscriptions(serverOptionsBuilder.ServiceProvider);
                })
                .AddMqttControllers()
                .AddMqttConnectionHandler()
                .AddMqttWebSocketServerAdapter()
                .AddConnections();

            services.AddSingleton<IFsMosquitoApp, FsMosquitoApp>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapConnectionHandler<MqttConnectionHandler>("/mqtt", options =>
                {
                    options.WebSockets.SubProtocolSelector = MqttSubProtocolSelector.SelectSubProtocol;
                });
            });

            app.UseSwagger();

            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "FSMosquito Desktop API V1");
                config.RoutePrefix = "api";
            });

            app.UseSpa(spa =>
            {
                spa.Options.PackageManagerCommand = "yarn";
                spa.Options.SourcePath = "../fsmosquito-app";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
            });

            app.UseMqttServer(server =>
            {
                var mqttService = app.ApplicationServices.GetRequiredService<IMqttService>();
                mqttService.ConfigureMqttServer(server);
            });

            app.UseFsMosquito();
        }
    }
}
