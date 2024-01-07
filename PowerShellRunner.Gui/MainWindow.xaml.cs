using EvilBaschdi.Core.Wpf;
using MahApps.Metro.Controls;
using PowerShellRunner.Core;
using PowerShellRunner.Gui.Internal;

namespace PowerShellRunner.Gui;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class MainWindow : MetroWindow
{
    /// <summary>
    ///     Constructor
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Load();
    }

    private void Load()
    {
        IApplicationStyle applicationStyle = new ApplicationStyle();
        IApplicationLayout applicationLayout = new ApplicationLayout();
        applicationStyle.Run();
        applicationLayout.RunFor((true, true));

        IScriptPaths scriptPaths = new ScriptPaths();
        IExecutePowerShellScript executePowerShellScript = new ExecutePowerShellScript();
        ITaskBarIconConfiguration taskBarIconConfiguration =
            new TaskBarIconConfiguration(this, PowerShellRunnerTaskBarIcon, executePowerShellScript, scriptPaths);
        taskBarIconConfiguration.StartMinimized();
        taskBarIconConfiguration.Run();

        //MessageBox.Show(stringBuilder.ToString());
    }
}