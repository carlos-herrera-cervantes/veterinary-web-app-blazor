using System;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Veterinary.Services.AuthServices;
using Veterinary.Services.EmployeeServices;
using Veterinary.Domain.Config;
using Veterinary.Services.CustomerServices;
using Veterinary.Services.PetServices;

namespace Veterinary.WebApp;

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
            c.BaseAddress = new Uri(AppConfig.GatewayHost);
        });
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IEmployeeService, EmployeeService>();
        services.AddTransient<IAvatarService, AvatarService>();
        services.AddTransient<ICustomerProfileService, CustomerProfileService>();
        services.AddTransient<ICustomerAvatarService, CustomerAvatarService>();
        services.AddTransient<IPetProfileService, PetProfileService>();
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
