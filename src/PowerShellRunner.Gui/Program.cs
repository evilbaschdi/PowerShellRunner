using Avalonia;
using EvilBaschdi.Core.Avalonia.AppBuilderImplementations;
using PowerShellRunner.Core.DependencyInjection;
using PowerShellRunner.Gui.DependencyInjection;

namespace PowerShellRunner.Gui;

internal class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => new AppBuilderImplementationToUseReactiveUIWithMicrosoftDependencyResolver<App>()
           .ValueFor(serviceCollection =>
                     {
                         serviceCollection.AddCoreServices();
                         serviceCollection.AddGuiServices();
                     })
#if DEBUG
           .WithDeveloperTools()
#endif
    ;
}