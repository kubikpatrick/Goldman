using Blazored.LocalStorage;

using Goldman.Web.Extensions;
using Goldman.Web.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Services;

namespace Goldman.Web;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddAuthorizationCore();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddGeolocationServices();
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<HtmlRenderer>();
        builder.Services.AddScoped<TokenService>();
        builder.Services.AddScoped<ServerUrlService>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        builder.Services.AddMudServices(configuration =>
        {
            configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
        });

        await builder.Build().RunAsync();
    }
}