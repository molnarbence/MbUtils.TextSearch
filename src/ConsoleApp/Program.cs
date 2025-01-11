using MbUtils.Extensions.CommandLineUtils;
using Core;
using ConsoleApp;
using Core.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var wrapper = new CommandLineApplicationWrapper<TextSearchApp>(args);

wrapper.HostBuilder.ConfigureAppConfiguration((context, builder) =>
{
    builder.AddJsonFile("appsettings.user.json", optional: true);
});

wrapper.HostBuilder.ConfigureServices((context, services) =>
{
    services.AddOptions<AppConfig>()
        .Bind(context.Configuration)
        .ValidateOnStart();

    services.AutoRegisterFromCore()
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
});

return await wrapper.ExecuteAsync();