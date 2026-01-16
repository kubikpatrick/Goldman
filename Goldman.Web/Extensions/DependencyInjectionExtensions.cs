using Goldman.Web.Handlers;
using Goldman.Web.Services;

namespace Goldman.Web.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddHttpClient(this IServiceCollection services)
    {
        return services.AddScoped(sp =>
        {
            var tokenService = sp.GetRequiredService<TokenService>();

            return new HttpClient(new JwtAuthorizationHandler(tokenService));
        });
    }
}