using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using GitWatch.Api.Options;
using GitWatch.DependencyInjection;
using Grace.AspNetCore.MVC;
using Grace.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            }).AddCookie()
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = Configuration["dfe59b98523cdd044f45"];
                    options.ClientSecret = Configuration["d9770ec38691199d7c550ad11cdca15ffc9994f4"];
                    options.CallbackPath = new PathString("/https://localhost:44332/swagger/index.html");

                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "23264520");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "Roman");
                    options.ClaimActions.MapJsonKey("urn:github:login", "LancerGreg");
                    options.ClaimActions.MapJsonKey("urn:github:url", "https://github.com/LancerGreg");
                    options.ClaimActions.MapJsonKey("urn:github:avatar", "https://avatars3.githubusercontent.com/u/23264520?v=4");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                            context.RunClaimActions(user);
                        }
                    };
                });
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
