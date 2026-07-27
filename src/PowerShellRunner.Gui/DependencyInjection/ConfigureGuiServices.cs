using Microsoft.Extensions.DependencyInjection;

//using PowerShellRunner.Core;

namespace PowerShellRunner.Gui.DependencyInjection;

/// <summary />
public static class ConfigureGuiServices
{
    /// <summary />
    public static void AddGuiServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IScriptDirectoryNodeScanner, ScriptDirectoryNodeScanner>();
        services.AddSingleton<ITrayIconMenuUpdater, TrayIconMenuUpdater>();
    }
}