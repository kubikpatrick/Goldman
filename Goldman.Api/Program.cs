using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using Goldman.Api.Data;
using Goldman.Api.Extensions;

namespace Goldman.Api;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        });
        builder.Services.AddMemoryCache();
        builder.Services.AddLogging();
        builder.Services.AddSignalR();
        builder.Services.AddIdentity();
        builder.Services.AddManagers();
        builder.Services.AddCors();
        builder.Services.AddTokenAuthentication(builder.Configuration);
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
        });
        
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHubs();

        await app.RunAsync();
    }
}
