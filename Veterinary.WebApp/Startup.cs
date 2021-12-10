using System;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Veterinary.Services.AuthServices;
using Veterinary.Services.EmployeeServices;

namespace Veterinary.WebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredLocalStorage();
            services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
            services.AddAuthorizationCore();
            services.AddHttpClient("veterinary", c =>
            {
                c.BaseAddress = new Uri("http://localhost:3000/api/v1/");
            });
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
