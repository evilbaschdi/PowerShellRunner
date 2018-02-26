using System.Windows;
using EvilBaschdi.Core.Internal;
using PowerShellRunner.Core;
using PowerShellRunner.Gui.Internal;

namespace PowerShellRunner.Gui
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            IMultiThreading multiThreading = new MultiThreading();
            IFileListFromPath fileListFromPath = new FileListFromPath(multiThreading);
            IScriptPaths scriptPaths = new ScriptPaths();
            IPowerShellScriptsRaw powerShellScriptsRaw = new PowerShellScriptsRaw(fileListFromPath, scriptPaths);
            IPowerShellScripts powerShellScripts = new PowerShellScripts(powerShellScriptsRaw);
            IExecutePowerShellScript executePowerShellScript = new ExecutePowerShellScript();
            ITaskbarIconConfiguration taskbarIconConfiguration = new TaskbarIconConfiguration(this, PowerShellRunnerTaskbarIcon, powerShellScripts, executePowerShellScript, scriptPaths);
            taskbarIconConfiguration.StartMinimized();
            taskbarIconConfiguration.Run();

            //MessageBox.Show(stringBuilder.ToString());
        }
    }
}