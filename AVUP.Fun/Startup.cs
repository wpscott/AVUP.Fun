using ClickHouse.Ado;
using ClickHouse.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace AVUP.Fun
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddClickHouse();
#if DEBUG
            services.AddTransient(p => new ClickHouseConnectionSettings("Host=100.100.100.1;Port=16028;Database=acfun;User=default"));
#else
            services.AddTransient(p => new ClickHouseConnectionSettings("Host=clickhouse-server;Port=9000;Database=acfun;User=default"));
#endif
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                    builder.SetPreflightMaxAge(TimeSpan.FromDays(30));
                });
            });
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
