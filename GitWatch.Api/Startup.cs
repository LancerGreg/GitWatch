using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitWatch.Api.Options;
using GitWatch.DependencyInjection;
using Grace.AspNetCore.MVC;
using Grace.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace GitWatch.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(service => { service.SwaggerDoc("v1", new Info { Title = "GitWatch API", Version = "v1" }); });
        }

        public void ConfigureContainer(IInjectionScope scope)
        {
            GraceContainer.ConfigureContainer(scope, Configuration);
            scope.SetupMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var swaggerOptions = new Options.SwaggerOptions();
            Configuration.GetSection(nameof(Options.SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(options => { options.RouteTemplate = swaggerOptions.JsonRoute; });
            app.UseSwaggerUI(options => { options.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description); });
;
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
