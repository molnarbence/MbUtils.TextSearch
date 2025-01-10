using MbUtils.Extensions.CommandLineUtils;
using MbUtils.TextSearch.Business;
using MbUtils.TextSearch.ConsoleHost;
using Microsoft.Extensions.DependencyInjection;

var wrapper = new CommandLineApplicationWrapper<TextSearchApp>(args);

wrapper.HostBuilder.ConfigureServices((context, services) =>
{
    services.AddOptions<AppConfig>()
        .Bind(context.Configuration)
        .ValidateOnStart();

    services.AutoRegisterFromMbUtilsTextSearchBusiness();
});

return await wrapper.ExecuteAsync();