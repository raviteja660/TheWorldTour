using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TheWorldTour.Services;
using TheWorld.Models;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorld.ViewModels;
using TheWorld.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            
            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                 OnRedirectToLogin = async ctx =>
                 {
                     if (ctx.Request.Path.StartsWithSegments("/api") &&
                     ctx.Response.StatusCode == 200)
                     {
                         ctx.Response.StatusCode = 401;
                     }else
                     {
                         ctx.Response.Redirect(ctx.RedirectUri);
                     }

                     await Task.Yield();
                 }
    
                };
            })
            .AddEntityFrameworkStores<WorldContext>();

            if (_env.IsEnvironment("Development") || _env.IsEnvironment("Testing"))
            {
                services.AddScoped<IMailService, DebugMailService>();
            }
            services.AddMvc(config =>
            {
                if (_env.IsProduction())
                {
                    config.Filters.Add(new RequireHttpsAttribute());
                }                
            })
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
            services.AddDbContext<WorldContext>();
            services.AddTransient<WorldContextSeedData>();
            services.AddTransient<GeoCoordsService>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env, 
            ILoggerFactory factory,
            WorldContextSeedData seeder)
        {
            Mapper.CreateMap<TripViewModel, Trip>().ReverseMap();
            Mapper.CreateMap<StopViewModel, Stop>().ReverseMap();

            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);
            }else
            {
                factory.AddDebug(LogLevel.Error);
            }

            //Order is important
            app.UseStaticFiles();
            app.UseIdentity();  
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{Controller}/{Action}/{id?}",
                    defaults: new { controller = "App", action = "Index" }
                    );
            });

            seeder.EnsureSeedData().Wait();
        }
    }
}
