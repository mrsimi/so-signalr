using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using so_signalr.Hubs;
using VueSPATemplate.Middleware;

namespace VueSPATemplate
{
    public class Startup
    {
        public const string CookieAuthScheme = "CookieAuthScheme";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            
            services.AddControllersWithViews();

            // In production, the Vue files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddAuthentication(CookieAuthScheme)
                .AddCookie(CookieAuthScheme, options => {
                    options.Cookie.Name = "soSignalR.AuthCookie";
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents{
                        OnRedirectToLogin = redirectContext => {
                            redirectContext.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask; 
                        }
                    };
                });

            services.AddSignalR();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<QuestionHub>("/question-hub");
            });

            // Instead of the default SPA approach where .NET proxies requests to the SPA server
            // This redirects "/" requests to the SPA server, who in turn proxies api requests to the .NET server
            // Avoids issues with the hot reload web sockets
            if (env.IsDevelopment())
            {
                app.UseVueDevelopmentServer();
            }
            else
            {
              app.UseSpa(spa =>
              {
                  spa.Options.SourcePath = "ClientApp";
              });
            }

          
        }
    }
}
