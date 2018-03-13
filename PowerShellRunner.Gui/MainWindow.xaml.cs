using MahApps.Metro.Controls;
using PowerShellRunner.Core;
using PowerShellRunner.Gui.Internal;

namespace PowerShellRunner.Gui
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
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
            IScriptPaths scriptPaths = new ScriptPaths();
            IExecutePowerShellScript executePowerShellScript = new ExecutePowerShellScript();
            ITaskbarIconConfiguration taskbarIconConfiguration = new TaskbarIconConfiguration(this, PowerShellRunnerTaskbarIcon, executePowerShellScript, scriptPaths);
            taskbarIconConfiguration.StartMinimized();
            taskbarIconConfiguration.Run();

            //MessageBox.Show(stringBuilder.ToString());
        }
    }
}