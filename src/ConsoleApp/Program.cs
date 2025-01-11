using MbUtils.Extensions.CommandLineUtils;
using Core;
using ConsoleApp;
using Microsoft.Extensions.DependencyInjection;

var wrapper = new CommandLineApplicationWrapper<TextSearchApp>(args);

wrapper.HostBuilder.ConfigureServices((context, services) =>
{
    services.AddOptions<AppConfig>()
        .Bind(context.Configuration)
        .ValidateOnStart();

    services.AutoRegisterFromCore();
});

return await wrapper.ExecuteAsync();