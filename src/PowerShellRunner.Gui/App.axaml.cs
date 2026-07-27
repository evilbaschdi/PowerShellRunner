using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using EvilBaschdi.Core.Avalonia.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PowerShellRunner.Core;

namespace PowerShellRunner.Gui;

/// <inheritdoc />
public class App : Application
{
    private readonly IExecutePowerShellScript _executePowerShellScript;
    private readonly IScriptDirectoryNodeScanner _scriptDirectoryNodeScanner;
    private readonly IScriptPaths _scriptPaths;
    private readonly ITrayIconMenuUpdater _trayIconMenuUpdater;

    /// <summary>
    ///     Constructor
    /// </summary>
    public App()
    {
        if (ApplicationServices.ServiceProvider == null)
        {
            throw new InvalidOperationException("Service provider is not initialized.");
        }

        _executePowerShellScript = ApplicationServices.ServiceProvider.GetRequiredService<IExecutePowerShellScript>();
        _scriptDirectoryNodeScanner = ApplicationServices.ServiceProvider.GetRequiredService<IScriptDirectoryNodeScanner>();
        _scriptPaths = ApplicationServices.ServiceProvider.GetRequiredService<IScriptPaths>();
        _trayIconMenuUpdater = ApplicationServices.ServiceProvider.GetRequiredService<ITrayIconMenuUpdater>();
    }

    /// <inheritdoc />
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        _trayIconMenuUpdater.EnsureStaticCloseMenuEntry(this, TrayIconMenuItemExitOnClick);
        _trayIconMenuUpdater.EnsureLoadingMenuEntry(this);
        _ = BuildScriptTrayIconMenuItemsAsync();

        base.OnFrameworkInitializationCompleted();
    }

    private async Task BuildScriptTrayIconMenuItemsAsync()
    {
        var dynamicRootNodes = await Task.Run(() => _scriptDirectoryNodeScanner.ValueFor(_scriptPaths.Value)).ConfigureAwait(false);
        await Dispatcher.UIThread.InvokeAsync(
            () => _trayIconMenuUpdater.ApplyRootNodes(this, dynamicRootNodes, _executePowerShellScript.RunFor),
            DispatcherPriority.Background);
    }

    private void TrayIconMenuItemExitOnClick(object sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}