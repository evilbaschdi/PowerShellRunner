using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using EvilBaschdi.Core.Extensions;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.IconPacks;
using PowerShellRunner.Core;

namespace PowerShellRunner.Gui.Internal
{
    public class TaskbarIconConfiguration : ITaskbarIconConfiguration
    {
        private readonly MainWindow _mainWindow;

        private readonly TaskbarIcon _taskbarIcon;
        private readonly IPowerShellScripts _powerShellScripts;
        private readonly IExecutePowerShellScript _executePowerShellScript;

        /// <summary>
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="taskbarIcon"></param>
        /// <param name="powerShellScripts"></param>
        /// <param name="executePowerShellScript"></param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="mainWindow" /> is <see langword="null" />.
        ///     <paramref name="taskbarIcon" /> is <see langword="null" />.
        /// </exception>
        public TaskbarIconConfiguration(MainWindow mainWindow, TaskbarIcon taskbarIcon, IPowerShellScripts powerShellScripts, IExecutePowerShellScript executePowerShellScript)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            _taskbarIcon = taskbarIcon ?? throw new ArgumentNullException(nameof(taskbarIcon));
            _powerShellScripts = powerShellScripts ?? throw new ArgumentNullException(nameof(powerShellScripts));
            _executePowerShellScript = executePowerShellScript ?? throw new ArgumentNullException(nameof(executePowerShellScript));
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        /// <summary>
        /// </summary>
        public void Run()
        {
            var filePath = Assembly.GetEntryAssembly().Location;
            _taskbarIcon.Icon = Icon.ExtractAssociatedIcon(filePath);
            _taskbarIcon.ContextMenu = TaskbarIconContextMenu();
            _taskbarIcon.TrayMouseDoubleClick += TaskbarIconDoubleClick;
        }

        /// <summary>
        /// </summary>
        public void StartMinimized()
        {
            _taskbarIcon.Visibility = Visibility.Visible;
            _mainWindow.Hide();
        }

        // ReSharper disable UseObjectOrCollectionInitializer
        private ContextMenu TaskbarIconContextMenu()
        {
            var contextMenu = new ContextMenu();

            var directoryMenuItems = new Dictionary<string, MenuItem>();

            foreach (var fileInfo in _powerShellScripts.Value.OrderBy(x => x.DirectoryName).ThenBy(x => x.Name))
            {
                var directory = fileInfo.Directory.GetProperDirectoryCapitalization();
                var file = fileInfo.GetProperFilePathCapitalization().Replace($"{directory}\\", "", StringComparison.InvariantCultureIgnoreCase);

                var directoryMenuItem = directoryMenuItems.FirstOrDefault(item => item.Key.Equals(directory)).Value
                                        ?? new MenuItem
                                           {
                                               Header = directory,
                                               Icon = new PackIconMaterial
                                                      {
                                                          Kind = PackIconMaterialKind.Folder
                                                      }
                                           };

                directoryMenuItems.Where(x => directory.StartsWith(x.Key) && directoryMenuItem.Parent == null).ToList().ForEach(y => y.Value.Items.Add(directoryMenuItem));

                if (!directoryMenuItems.ContainsKey(directory))
                {
                    directoryMenuItems.Add(directory, directoryMenuItem);
                }

                var fileMenuItem = new MenuItem
                                   {
                                       Header = file,
                                       Icon = new PackIconMaterial
                                              {
                                                  Kind = PackIconMaterialKind.Script
                                              },
                                       StaysOpenOnClick = false
                                   };
                fileMenuItem.Click += (sender, args) => _executePowerShellScript.RunFor(fileInfo);

                directoryMenuItem.Items.Add(fileMenuItem);
            }

            directoryMenuItems.Values.Where(x => x.Parent == null).ToList().ForEach(item => contextMenu.Items.Add(item));

            var restoreApplication = new MenuItem
                                     {
                                         Header = "Restore application",
                                         Icon = new PackIconMaterial
                                                {
                                                    Kind = PackIconMaterialKind.WindowRestore
                                                }
                                     };
            restoreApplication.Click += ContextMenuItemRestoreClick;

            var closeApplication = new MenuItem
                                   {
                                       Header = "Close application",
                                       Icon = new PackIconMaterial
                                              {
                                                  Kind = PackIconMaterialKind.Power
                                              }
                                   };
            closeApplication.Click += ContextMenuItemCloseClick;

            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(restoreApplication);
            contextMenu.Items.Add(closeApplication);

            return contextMenu;
        }

        private void ContextMenuItemCloseClick(object sender, EventArgs e)
        {
            _taskbarIcon.Dispose();
            _mainWindow.Close();
        }

        private void ContextMenuItemRestoreClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }

        private void TaskbarIconDoubleClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }
    }
}