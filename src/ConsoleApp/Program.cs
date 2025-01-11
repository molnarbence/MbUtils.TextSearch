using MbUtils.Extensions.CommandLineUtils;
using Core;
using ConsoleApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    services.AutoRegisterFromCore();
});

return await wrapper.ExecuteAsync();