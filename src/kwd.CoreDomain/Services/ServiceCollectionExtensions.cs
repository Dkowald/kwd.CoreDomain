using Microsoft.Extensions.DependencyInjection;

namespace kwd.CoreDomain.Services;

/// <summary>
/// Extensions 
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add the default <see cref="IClock"/> singleton to the services.
    /// </summary>
    public static IServiceCollection AddClock(this IServiceCollection services)
        => services.AddSingleton<IClock, Clock>();
}