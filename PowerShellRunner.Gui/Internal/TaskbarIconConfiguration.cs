using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.IconPacks;
using PowerShellRunner.Core;

namespace PowerShellRunner.Gui.Internal
{
    /// <inheritdoc />
    public class TaskBarIconConfiguration : ITaskBarIconConfiguration
    {
        private readonly IExecutePowerShellScript _executePowerShellScript;
        private readonly MainWindow _mainWindow;
        private readonly IScriptPaths _scriptPaths;
        private readonly TaskbarIcon _taskBarIcon;

        /// <summary>
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="taskBarIcon"></param>
        /// <param name="executePowerShellScript"></param>
        /// <param name="scriptPaths"></param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="mainWindow" /> is <see langword="null" />.
        ///     <paramref name="taskBarIcon" /> is <see langword="null" />.
        /// </exception>
        public TaskBarIconConfiguration(MainWindow mainWindow, TaskbarIcon taskBarIcon,
                                        IExecutePowerShellScript executePowerShellScript,
                                        IScriptPaths scriptPaths)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            _taskBarIcon = taskBarIcon ?? throw new ArgumentNullException(nameof(taskBarIcon));

            _executePowerShellScript = executePowerShellScript ??
                                       throw new ArgumentNullException(nameof(executePowerShellScript));
            _scriptPaths = scriptPaths ?? throw new ArgumentNullException(nameof(scriptPaths));
        }


        /// <inheritdoc />
        public void Run()
        {
            var filePath = typeof(MainWindow).Assembly.Location;
            _taskBarIcon.Icon = Icon.ExtractAssociatedIcon(filePath);
            _taskBarIcon.ContextMenu = TaskBarIconContextMenu();
            _taskBarIcon.TrayMouseDoubleClick += TaskBarIconDoubleClick;
        }

        /// <summary>
        /// </summary>
        public void StartMinimized()
        {
            _taskBarIcon.Visibility = Visibility.Visible;
            _mainWindow.Hide();
        } // ReSharper disable UseObjectOrCollectionInitializer
        private ContextMenu TaskBarIconContextMenu()
        {
            var contextMenu = new ContextMenu();

            foreach (var scriptPath in _scriptPaths.Value)
            {
                var info = new DirectoryInfo(scriptPath);
                if (!info.Exists)
                {
                    continue;
                }

                var rootNode = new MenuItem
                               {
                                   Header = info.Name,
                                   Icon = new PackIconMaterial
                                          {
                                              Kind = PackIconMaterialKind.Folder
                                          },
                                   Tag = info
                               };
                GetFiles(info, rootNode);
                GetDirectories(info.GetDirectories(), rootNode);
                contextMenu.Items.Add(rootNode);
            }

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

        private void GetDirectories(IEnumerable<DirectoryInfo> subDirs, ItemsControl nodeToAddTo)
        {
            foreach (var subDir in subDirs.Where(dir => !dir.Name.Equals("packages") || !dir.Name.Equals("Modules")))
            {
                var aNode = new MenuItem
                            {
                                Header = subDir.Name,
                                Icon = new PackIconMaterial
                                       {
                                           Kind = PackIconMaterialKind.Folder
                                       },
                                Tag = subDir
                            };

                var subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }

                GetFiles(subDir, aNode);
                if (aNode.HasItems)
                {
                    nodeToAddTo.Items.Add(aNode);
                }
            }
        }

        private void GetFiles(DirectoryInfo directoryInfo, ItemsControl nodeToAddTo)
        {
            foreach (var fileInfo in directoryInfo.GetFiles("*.ps1"))
            {
                var fNode = new MenuItem
                            {
                                Header = fileInfo.Name,
                                Icon = new PackIconMaterial
                                       {
                                           Kind = PackIconMaterialKind.Script
                                       },
                                Tag = fileInfo
                            };
                fNode.Click += (_, _) => _executePowerShellScript.RunFor(fileInfo);
                nodeToAddTo.Items.Add(fNode);
            }
        }

        private void ContextMenuItemCloseClick(object sender, EventArgs e)
        {
            _taskBarIcon.Dispose();
            _mainWindow.Close();
        }

        private void ContextMenuItemRestoreClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }

        private void TaskBarIconDoubleClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }
    }
}