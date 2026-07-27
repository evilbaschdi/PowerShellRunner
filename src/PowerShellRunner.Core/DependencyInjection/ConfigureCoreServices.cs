using Microsoft.Extensions.DependencyInjection;

namespace PowerShellRunner.Core.DependencyInjection;

/// <summary />
public static class ConfigureCoreServices
{
    /// <summary />
    public static void AddCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IExecutePowerShellScript, ExecutePowerShellScript>();
        services.AddSingleton<IPowerShellExecutableCandidates, PowerShellExecutableCandidates>();
        services.AddSingleton<IPowerShellProcessStarter, PowerShellProcessStarter>();
        services.AddSingleton<IScriptPaths, ScriptPaths>();
    }
}