using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CoreWithAngular2
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.Use(
                async (context, next) =>
                {
                    await next();

                    // If there is no available file and request doesn't contain an extension,
                    // assume we are trying to access a page.  Rewrite the request to use the app root.
                    if (context.Response.StatusCode == 404 
                        && !Path.HasExtension(context.Request.Path.Value)
                        && !context.Request.Path.Value.StartsWith("api")
                    )
                    {
                        context.Request.Path = "/index.html";
                        context.Response.StatusCode = 200; // Make sure we update the status code, otherwise we will get a 404 return.
                        await next();
                    }
                }

                );

            app.UseMvc(config =>
                config.MapRoute(
                  name: "Default",
                  template: "{controller}/{Action}/{id?}",
                  defaults: new { controller = "App", action = "Index" }
                )
            );

            app.UseMvc();
        }
    }
}
