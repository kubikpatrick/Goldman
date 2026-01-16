using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using Goldman.Api.Data;
using Goldman.Api.Hubs;
using Goldman.Api.Services;
using Goldman.Api.Services.Managers;
using Goldman.Models.Identity;

namespace Goldman.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<DeviceManager>();
        services.AddScoped<RefreshTokenManager>();
        
        return services;
    }

    public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<JwtService>();
        services.AddAuthentication(options => 
        { 
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
            };
            
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Query[Constants.AccessToken];
                    var path = context.HttpContext.Request.Path;
                    
                    if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = token;
                    }
                    
                    return Task.CompletedTask;
                },
                
                OnAuthenticationFailed = context => Task.CompletedTask,
            };
        });
        
        return services;
    }

    public static WebApplication UseCors(this WebApplication app)
    {
        app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        
        return app;
    }

    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<LocationHub>("/hubs/location");

        return app;
    }
}