using Catalog.API.Data;
using Catalog.API.Repositories;
using Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API
{
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
            var deltaBackOffms = Convert.ToInt32(TimeSpan.FromSeconds(5).TotalMilliseconds);
            var maxdeltaBackOffms = Convert.ToInt32(TimeSpan.FromSeconds(20).TotalMilliseconds);


            var options = new ConfigurationOptions
            {

                EndPoints = { $"{Configuration.GetValue<string>("RedisCache:Host")}:{Configuration.GetValue<int>("RedisCache:Port")}" },
                ConnectRetry = 5,
                ReconnectRetryPolicy = new ExponentialRetry(deltaBackOffms,
                                                maxdeltaBackOffms),
                ConnectTimeout = 1000,
                AbortOnConnectFail = false,
                SyncTimeout = 10000
            };

            var redisMultiplexer = ConnectionMultiplexer.Connect(options);

            services.AddSingleton<IConnectionMultiplexer>(redisMultiplexer);

            /*services.AddSingleton<IConnectionMultiplexer>(opt =>
                   ConnectionMultiplexer.Connect(Configuration.GetValue<string>("CacheSettings:ConnectionString")));*/

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetValue<string>("CacheSettings:ConnectionString");
            });

            services.AddScoped<ICacheService, CacheService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" });
            });

            services.AddScoped<ICatalogContext, CatalogContext>();
            services.AddScoped<IProductRepository , ProductRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
