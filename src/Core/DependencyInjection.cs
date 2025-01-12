using Core.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {   
        services
            .AddSingleton<IResultRepository, FileBasedResultRepository>()
            .AddSingleton<MainLogic>()
            .AddSingleton<ISearchTermCounterStrategy>(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<AppConfig>>();
                return config.Value.Strategy switch
                {
                    "Regex" => new RegexStrategy(),
                    "KnuthMorrisPratt" => new KnuthMorrisPratt(),
                    "StringSplit" => new StringSplitStrategy(),
                    _ => throw new NotSupportedException($"Strategy '{config.Value.Strategy}' is not supported.")
                };
            });
        
        return services;
    }
}