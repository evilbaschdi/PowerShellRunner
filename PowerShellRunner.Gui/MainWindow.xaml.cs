using EvilBaschdi.CoreExtended.Metro;
using MahApps.Metro.Controls;
using PowerShellRunner.Core;
using PowerShellRunner.Gui.Internal;

namespace PowerShellRunner.Gui
{
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
            var themeManagerHelper = new ThemeManagerHelper();
            var applicationStyle = new ApplicationStyle(themeManagerHelper);
            applicationStyle.Load();

            IScriptPaths scriptPaths = new ScriptPaths();
            IExecutePowerShellScript executePowerShellScript = new ExecutePowerShellScript();
            ITaskBarIconConfiguration taskBarIconConfiguration =
                new TaskBarIconConfiguration(this, PowerShellRunnerTaskBarIcon, executePowerShellScript, scriptPaths);
            taskBarIconConfiguration.StartMinimized();
            taskBarIconConfiguration.Run();

            //MessageBox.Show(stringBuilder.ToString());
        }
    }
}